using Application.Common.IoC;
using Domain.Model.Quotes;
using Domain.Model.Quotes.Abstractions;
using Infrastructure.Common.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infrastructure.Contexts.Quotes.Repositories
{
    [Ioc(typeof(IQuoteRepository))]
    internal class QuoteRepository : IQuoteRepository
    {


        private readonly ApplicationDbContext context;


        public QuoteRepository(ApplicationDbContext applicationDbContext)
        {
            this.context = applicationDbContext;
        }


        public async Task<Quote?> Get(int quoteId, CancellationToken cancellationToken, bool tracked = false)
        {
            var query = context.Quotes
                .Include(quote => quote.Author)
                .Include(quote => quote.Episode)
                .Include(quote => quote.Episode.Arc)
                .Include(quote => quote.Episode.Arc.Saga)
                .AsQueryable();

            if (!tracked)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(quote => quote.Id == quoteId, cancellationToken);
        }



        public async Task<IEnumerable<Quote>> List(
            int? authorId,
            int? arcId,
            string? searchTerm,
            bool? isReviewed,
            CancellationToken cancellationToken)
        {
            var query = context.Quotes
                .Include(q => q.Author)
                .Include(q => q.Episode)
                .Include(q => q.Episode.Arc)
                .Include(q => q.Episode.Arc.Saga)
                .AsQueryable();

            if (authorId.HasValue)
            {
                query = query.Where(q => q.AuthorId == authorId.Value);
            }

            if (arcId.HasValue)
            {
                query = query.Where(q => q.Episode != null && q.Episode.ArcId == arcId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(q =>
                    q.Text.Contains(searchTerm) ||
                    q.Translations.Any(translation =>
                        translation.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName &&
                        translation.Value.Contains(searchTerm)));
            }

            if (isReviewed.HasValue)
            {
                query = query.Where(q => q.IsReviewed == isReviewed.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }



        public async Task Save(Quote quote, CancellationToken cancellationToken)
        {
            if (quote.Id == 0)
            {
                await context.Quotes.AddAsync(quote, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
        }


        public async Task Delete(int quoteId, CancellationToken cancellationToken)
        {
            await context.Quotes.Where(quote => quote.Id == quoteId).ExecuteDeleteAsync(cancellationToken);
        }


    }
}
