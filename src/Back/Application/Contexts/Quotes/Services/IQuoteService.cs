using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;

namespace Application.Contexts.Quotes.Services
{
    public interface IQuoteService
    {


        public Task Save(QuoteDTO quote, CancellationToken cancellationToken);
        public Task Delete(int quoteId, CancellationToken cancellationToken);
    }
}
