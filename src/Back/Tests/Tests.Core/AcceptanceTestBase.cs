using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tests.Core
{
    public abstract class AcceptanceTestBase<T> : TestBase where T : class
    {


        static private WebApplicationFactory<T> webApplicationFactory;
        static private HttpClient webApplicationClient;


        protected WebApplicationFactory<T> WebApplicationFactory => AcceptanceTestBase<T>.webApplicationFactory
            ??= new WebApplicationFactory<T>().WithWebHostBuilder(builder =>
            {
                ConfigureServices(builder);
            });


        protected virtual void ConfigureServices(IWebHostBuilder builder)
        {

        }


        protected HttpClient WebApplicationClient => webApplicationClient ??= WebApplicationFactory.CreateClient();
    }
}
