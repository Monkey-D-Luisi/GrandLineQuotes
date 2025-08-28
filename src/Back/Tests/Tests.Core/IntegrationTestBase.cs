using Infrastructure.Common.DatabaseContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Tests.Core
{
    public class IntegrationTestBase : TestBase
    {


        protected ApplicationDbContext dbContext;
        protected IServiceScope scope;


        protected virtual void ConfigureServices(IHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddEnvironmentVariables();
            });

            builder.ConfigureServices(services => 
            {
                services.AddPooledDbContextFactory<ApplicationDbContext>((serviceProvider, options) =>
                {
                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                    options.UseMySql(
                        configuration.GetConnectionString("grandlinequotes"),
                        ServerVersion.AutoDetect(configuration.GetConnectionString("grandlinequotes")),
                        sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 1,
                                maxRetryDelay: TimeSpan.FromSeconds(1),
                                errorNumbersToAdd: null);
                            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        });

                    options.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
                    options.EnableSensitiveDataLogging(true);
                    options.EnableDetailedErrors(true);
                });

                services.AddScoped<ApplicationDbContext>(sp =>
                {
                    var factory = sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
                    return factory.CreateDbContext();
                });
            });
        }


        [SetUp]
        public void Setup()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    Console.WriteLine($"🔍 Environment detected: {context.HostingEnvironment.EnvironmentName}");
                });
            ConfigureServices(builder);
            var host = builder.Build();
            scope = host.Services.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }


        [TearDown]
        public void TearDown()
        {
            dbContext?.Dispose();
            scope?.Dispose();
        }
    }
}
