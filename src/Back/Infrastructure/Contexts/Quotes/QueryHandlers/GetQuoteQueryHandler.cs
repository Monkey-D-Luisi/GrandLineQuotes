using Application.Contexts.Quotes.Queries;
using Domain.Model.Quotes;
using Domain.Model.Quotes.Abstractions;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using MediatR;

namespace Infrastructure.Contexts.Quotes.QueryHandlers
{
    internal class GetQuoteQueryHandler : IRequestHandler<GetQuoteQuery, QuoteDTO>
    {


        private readonly IQuoteRepository repository;


        public GetQuoteQueryHandler(IQuoteRepository repository)
        {
            this.repository = repository;
        }


        public async Task<QuoteDTO> Handle(GetQuoteQuery request, CancellationToken cancellationToken)
        {
            Quote? quote = await repository.Get(request.QuoteId, cancellationToken);

            return quote?.GetSnapshot() ?? new QuoteDTO();
        }
    }
}
