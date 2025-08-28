using Dawn;
using MediatR;

namespace Application.Contexts.Quotes.Commands
{
    public class DeleteQuoteCommand : IRequest
    {


        public DeleteQuoteCommand(int quoteId)
        {
            Guard.Argument(quoteId, nameof(quoteId)).Positive();

            QuoteId = quoteId;
        }


        public int QuoteId { get; }
    }
}
