using Application.Common.IoC;
using Domain.Model.Episodes;
using Domain.Model.Episodes.Repositories;
using Infrastructure.Common.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts.Episodes.Repositories
{
    [Ioc(typeof(IEpisodeRepository))]
    internal class EpisodeRepository : IEpisodeRepository
    {


        private readonly ApplicationDbContext context;


        private bool disposed = false;


        public EpisodeRepository(ApplicationDbContext applicationDbContext)
        {
            this.context = applicationDbContext;
        }


        public async Task<Episode?> Get(int number, CancellationToken cancellationToken, bool tracked = false)
        {
            var query = context.Episodes.AsQueryable();

            if (!tracked)
                query = query.AsNoTracking();

            return await query
                .FirstOrDefaultAsync(episode => episode.Number == number, cancellationToken);
        }


        public async Task<IEnumerable<Episode>> List()
        {
            return await context.Episodes
                .Include(episode => episode.Arc)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task Create(Episode episode, CancellationToken cancellationToken)
        {
            await context.Episodes.AddAsync(episode, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }


        public async Task Update(Episode episode, CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken);
        }


        public async Task Delete(int number, CancellationToken cancellationToken)
        {
            await context.Episodes.Where(episode => episode.Number == number).ExecuteDeleteAsync(cancellationToken);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
    }
}
