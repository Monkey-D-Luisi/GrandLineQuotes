using Application.Common.IoC;
using Domain.Model.Characters;
using Domain.Model.Characters.Abstractions;
using Infrastructure.Common.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts.Characters.Repositories
{
    [Ioc(typeof(ICharacterRepository))]
    internal class CharacterRepository : ICharacterRepository
    {


        private readonly ApplicationDbContext context;


        private bool disposed = false;


        public CharacterRepository(ApplicationDbContext applicationDbContext)
        {
            context = applicationDbContext;
        }


        public async Task<Character?> Get(int id, CancellationToken cancellationToken, bool tracked = false)
        {
            var query = context.Characters.AsQueryable();

            if (!tracked)
                query = query.AsNoTracking();

            return await query
                .FirstOrDefaultAsync(character => character.Id == id, cancellationToken);
        }


        public async Task<IEnumerable<Character>> List(CancellationToken cancellationToken)
        {
            return await context.Characters
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }


        public async Task Save(Character character, CancellationToken cancellationToken)
        {
            if (character.Id == 0)
            {
                await context.Characters.AddAsync(character, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
        }


        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await context.Characters.Where(character => character.Id == id).ExecuteDeleteAsync(cancellationToken);
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
