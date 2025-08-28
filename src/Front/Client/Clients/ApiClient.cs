using Client.Abstractions.Clients;
using Flurl;
using Flurl.Http;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using GrandLineQuotes.Client.Abstractions.RequestModels.Quotes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Client.Clients
{
    public class ApiClient : IApiClient
    {


        private readonly string apiHttpHost;
        private readonly IOptions<ApiClientOptions> options;


        public ApiClient(
            IConfiguration config,
            IOptions<ApiClientOptions> options)
        {
            this.options = options;

            if (this.options?.Value == null)
            {
                var apiOptions = new ApiClientOptions();
                this.options = Options.Create(apiOptions);
            }

            apiHttpHost = config.GetSection(this.options.Value.HttpHostConfigKey).Value ?? string.Empty;
        }

        public async Task<IEnumerable<QuoteDTO>> ListQuotes(QuotesListRequestModel requestModel)
        {
            var endpoint =
                apiHttpHost
                .AppendPathSegment("v1/quotes")
                .SetQueryParams(requestModel);

            return await endpoint
                .GetJsonAsync<IEnumerable<QuoteDTO>>();
        }


        public async Task<QuoteDTO> GetQuote(int quoteId)
        {
            var endpoint =
                apiHttpHost
                .AppendPathSegment("v1/quotes")
                .AppendPathSegment(quoteId);

            return await endpoint
                .GetJsonAsync<QuoteDTO>();
        }
    }
}
