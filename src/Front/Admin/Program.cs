using Application.Common.Exceptions.Filters;
using Application.Common.IoC;
using Infrastructure.Common.DatabaseContexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyModel;
using Minio;
using System.Globalization;
using System.Reflection;
using System.Runtime.Loader;


namespace Admin
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<GlobalExceptionFilter>();
            builder.Services.AddControllersWithViews(options =>
            {
                if (
                    !builder.Environment.IsDevelopment() &&
                    !builder.Environment.IsEnvironment("Local") &&
                    !builder.Environment.IsEnvironment("Test") &&
                    !builder.Environment.IsEnvironment("Sandbox"))
                {
                    options.Filters.Add(new AuthorizeFilter());
                }
                options.Filters.Add<GlobalExceptionFilter>();
            });

            if (
                !builder.Environment.IsDevelopment() &&
                !builder.Environment.IsEnvironment("Local") &&
                !builder.Environment.IsEnvironment("Test") &&
                !builder.Environment.IsEnvironment("Sandbox"))
            {
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "Google";
                })
                .AddCookie()
                .AddOpenIdConnect("Google", options =>
                {
                    options.Authority = builder.Configuration.GetValue<string>("Authentication:Google:Authority");
                    options.ClientId = builder.Configuration.GetValue<string>("Authentication:Google:ClientId");
                    options.ClientSecret = builder.Configuration.GetValue<string>("Authentication:Google:ClientSecret");
                    options.CallbackPath = "/signin-google";
                    options.SaveTokens = true;

                    options.Scope.Add("email");
                    options.Events = new OpenIdConnectEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var allowedEmail = builder.Configuration.GetValue<string>("Authentication:Google:AllowedEmail");
                            var emailClaim = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

                            if (emailClaim != allowedEmail)
                            {
                                context.Fail("Denied access.");
                            }

                            return Task.CompletedTask;

                        },
                        OnRedirectToIdentityProvider = context =>
                        {
                            if (!context.ProtocolMessage.RedirectUri.StartsWith("https://"))
                            {
                                var uriBuilder = new UriBuilder(context.ProtocolMessage.RedirectUri)
                                {
                                    Scheme = "https",
                                    Port = -1
                                };
                                context.ProtocolMessage.RedirectUri = uriBuilder.ToString();
                            }

                            return Task.CompletedTask;
                        },
                    };
                });
            }

            builder.Services.AddAuthorization();

            builder.Services
                .AddPooledDbContextFactory<ApplicationDbContext>((serviceProvider, options) =>
                {
                    options.UseMySql(
                        builder.Configuration.GetConnectionString("grandlinequotes"),
                        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("grandlinequotes")),
                        sqlOptions =>
                        {
                            sqlOptions
                                .EnableRetryOnFailure(
                                    maxRetryCount: 1,
                                    maxRetryDelay: TimeSpan.FromSeconds(1),
                                    errorNumbersToAdd: null);

                            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        });

                    options.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
                    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
                    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
                });

            builder.Services.AddScoped<ApplicationDbContext>(sp =>
            {
                var factory = sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
                return factory.CreateDbContext();
            });

            builder.Services.AddMinio(configureClient =>
            {
                configureClient
                    .WithEndpoint(builder.Configuration.GetValue<string>("Minio:Endpoint"))
                    .WithCredentials(
                        builder.Configuration.GetValue<string>("Minio:AccessKey"),
                        builder.Configuration.GetValue<string>("Minio:SecretKey")
                    )
                    .WithSSL(builder.Configuration.GetValue<bool>("Minio:Secure"))
                    .Build();
            });

            // Límite de formularios (multipart)
            builder.Services.Configure<FormOptions>(o =>
            {
                o.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
            });

            // Límite del cuerpo de la petición en Kestrel
            builder.WebHost.ConfigureKestrel(o =>
            {
                o.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100 MB
            });

            var dependencies = DependencyContext.Default?
                .RuntimeLibraries
                .Where(asm => asm.Name == "Infrastructure" || asm.Name == "Application")
                ?? Enumerable.Empty<RuntimeLibrary>();
            var assemblies = dependencies.Select(library => AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(library.Name)));

            foreach (var assembly in assemblies)
            {
                var typesWithAttribute = assembly.GetTypes()
                    .Where(t => t.GetCustomAttribute<IocAttribute>() != null);

                foreach (var type in typesWithAttribute)
                {
                    var attribute = type.GetCustomAttribute<IocAttribute>();
                    if (attribute != null && attribute.ServiceType != null)
                    {
                        builder.Services.AddScoped(attribute.ServiceType, type);
                    }
                }
            }

            builder.Services.AddMediatR(cfg =>
            {
                foreach (var assembly in assemblies)
                {
                    cfg.RegisterServicesFromAssembly(assembly);
                }
                cfg.LicenseKey = builder.Configuration.GetValue<string>("LuckyPenny:LicenseKey");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                if (!app.Environment.IsEnvironment("Test") && !app.Environment.IsEnvironment("Sandbox"))
                {
                    app.UseHsts();
                }
            }

            app.UseStaticFiles();

            app.UseRouting();

            if (
                !app.Environment.IsDevelopment() &&
                !app.Environment.IsEnvironment("Local") &&
                !app.Environment.IsEnvironment("Test") &&
                !app.Environment.IsEnvironment("Sandbox"))
            {
                app.UseAuthentication();
            }

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Use(async (context, next) =>
            {
                var lang = context.Request.Headers["Accept-Language"].FirstOrDefault()?.Split(",").FirstOrDefault() ?? "en";
                try
                {
                    var culture = new CultureInfo(lang);
                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                }
                catch
                {
                    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                    CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
                }
                await next.Invoke();
            });

            app.Run();
        }
    }
}