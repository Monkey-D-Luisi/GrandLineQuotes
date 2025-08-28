using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace Tests.Core
{
    internal class TestCustomHttpClientFactory : DefaultFlurlClientFactory
    {


        private readonly HttpClient httpClient;


        public TestCustomHttpClientFactory(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }


        protected override IFlurlClient Create(Url url)
        {
            if (httpClient != null && url.Path.Contains("localhost"))
                return new FlurlClient(httpClient);

            return base.Create(url);
        }
    }
}
