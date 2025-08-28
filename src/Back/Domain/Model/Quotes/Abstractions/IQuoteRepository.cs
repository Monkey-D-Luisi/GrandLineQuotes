using Domain.Model.Quotes;

namespace Domain.Model.Quotes.Abstractions
{
    public interface IQuoteRepository
    {


        public Task<Quote?> Get(int quoteId, CancellationToken cancellationToken, bool tracked = false);
        public Task<IEnumerable<Quote>> List(int? authorId, int? arcId, string? searchTerm, bool? isReviewed = true, CancellationToken cancellationToken = default);
        public Task Save(Quote quote, CancellationToken cancellationToken);
        public Task Delete(int quoteId, CancellationToken cancellationToken);
    }
}
