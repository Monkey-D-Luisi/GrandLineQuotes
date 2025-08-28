using Application.Contexts.Quotes.Queries;
using Domain.Model.Quotes;
using Domain.Model.Quotes.Abstractions;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using MediatR;

namespace Infrastructure.Contexts.Quotes.QueryHandlers
{
    internal class ListQuotesQueryHandler : IRequestHandler<ListQuotesQuery, IEnumerable<QuoteDTO>>
    {


        private readonly IQuoteRepository repository;


        public ListQuotesQueryHandler(IQuoteRepository repository)
        {
            this.repository = repository;
        }


        public async Task<IEnumerable<QuoteDTO>> Handle(ListQuotesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Quote> quotesList = await repository.List(
                request.AuthorId,
                request.ArcId,
                request.SearchTerm,
                request.IsReviewed,
                cancellationToken
                );

            return quotesList?.Select(quote => quote.GetSnapshot()) ?? new List<QuoteDTO>();
        }
    }
}
