using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using MediatR;

namespace Application.Contexts.Quotes.Queries
{
    public class ListQuotesQuery : IRequest<IEnumerable<QuoteDTO>>
    {


        public ListQuotesQuery(int? authorId, int? arcId, string? searchTerm, bool? isReviewed = true)
        {
            AuthorId = authorId;
            ArcId = arcId;
            SearchTerm = searchTerm;
            IsReviewed = isReviewed;
        }


        public int? AuthorId { get; }
        public int? ArcId { get; }
        public string? SearchTerm { get; }
        public bool? IsReviewed { get; }
    }
}
