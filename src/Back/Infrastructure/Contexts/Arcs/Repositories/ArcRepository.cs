using Application.Common.IoC;
using Domain.Model.Arcs;
using Domain.Model.Arcs.Repositories;
using Infrastructure.Common.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts.Arcs.Repositories
{
    [Ioc(typeof(IArcRepository))]
    internal class ArcRepository : IArcRepository
    {


        private readonly ApplicationDbContext context;


        private bool disposed = false;


        public ArcRepository(ApplicationDbContext context)
        {
            this.context = context;
        }


        public async Task<Arc?> Get(int? id, CancellationToken cancellationToken, bool tracked = false)
        {
            var query = context.Arcs.AsQueryable();

            if (!tracked)
                query = query.AsNoTracking();

            return await query
                .FirstOrDefaultAsync(arc => arc.Id == id, cancellationToken);
        }


        public async Task<IEnumerable<Arc>> List(CancellationToken cancellationToken)
        {
            return await context.Arcs
                .Include(arc => arc.Saga)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }


        public async Task Save(Arc arc, CancellationToken cancellationToken)
        {
            if (arc.Id == 0)
            {
                await context.Arcs.AddAsync(arc, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await context.Arcs.Where(arc => arc.Id == id).ExecuteDeleteAsync(cancellationToken);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }
    }
}
