using Client.Clients.Extensions;
using Flurl.Http;
using Microsoft.AspNetCore.Hosting;

namespace Tests.Core
{
    public abstract class ClientAcceptanceTestBase<T> : AcceptanceTestBase<T> where T : class
    {


        [SetUp]
        public new void SetUp()
        {
            FlurlHttp.GlobalSettings.ResetDefaults();
            FlurlHttp.Configure(settings =>
            {
                settings.FlurlClientFactory = new TestCustomHttpClientFactory(WebApplicationClient);
                settings.Timeout = TimeSpan.FromSeconds(10);
            });
        }


        protected override void ConfigureServices(IWebHostBuilder builder)
        {
            base.ConfigureServices(builder);

            builder.UseSetting("Api:HttpHost", "http://localhost");

            builder.ConfigureServices((context, services) =>
            {
                services.AddApiClient();
            });
        }
    }
}
