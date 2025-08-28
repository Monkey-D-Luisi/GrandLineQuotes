using Dawn;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using MediatR;

namespace Application.Contexts.Quotes.Queries
{
    public class GetQuoteQuery : IRequest<QuoteDTO>
    {


        public GetQuoteQuery(int quoteId)
        {
            Guard.Argument(quoteId, nameof(quoteId)).Positive();

            QuoteId = quoteId;
        }


        public int QuoteId { get; }
    }
}
