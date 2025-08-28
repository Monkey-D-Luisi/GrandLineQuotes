using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using GrandLineQuotes.Client.Abstractions.RequestModels.Quotes;

namespace Client.Abstractions.Clients
{
    public interface IApiClient
    {


        Task<IEnumerable<QuoteDTO>> ListQuotes(QuotesListRequestModel requestModel);
        Task<QuoteDTO> GetQuote(int quoteId);
    }
}
