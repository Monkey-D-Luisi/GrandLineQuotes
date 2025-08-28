using Application.Common.Exceptions.Filters;
using Application.Common.IoC;
using Infrastructure.Common.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyModel;
using Minio;
using System.Reflection;
using System.Runtime.Loader;

namespace Api {
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddScoped<GlobalExceptionFilter>();
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
            if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local") || app.Environment.IsEnvironment("Test"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}