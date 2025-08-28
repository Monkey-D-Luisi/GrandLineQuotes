using Application.Common.IoC;
using Domain.Model.Sagas;
using Domain.Model.Sagas.Repositories;
using Infrastructure.Common.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts.Sagas.Repositories
{
    [Ioc(typeof(ISagaRepository))]
    internal class SagaRepository : ISagaRepository
    {


        private readonly ApplicationDbContext context;


        private bool disposed = false;


        public SagaRepository(ApplicationDbContext context)
        {
            this.context = context;
        }


        public async Task<Saga?> Get(int? id, CancellationToken cancellationToken, bool tracked = false)
        {
            var query = context.Sagas.AsQueryable();

            if (!tracked)
                query = query.AsNoTracking();

            return await query
                .FirstOrDefaultAsync(saga => saga.Id == id, cancellationToken);
        }


        public async Task<IEnumerable<Saga>> List(CancellationToken cancellationToken)
        {
            return await context.Sagas
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }


        public async Task Save(Saga saga, CancellationToken cancellationToken)
        {
            if (saga.Id == 0)
            {
                await context.Sagas.AddAsync(saga, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await context.Sagas.Where(saga => saga.Id == id).ExecuteDeleteAsync(cancellationToken);
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
