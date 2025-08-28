
namespace Domain.Model.Characters.Abstractions
{
    public interface ICharacterRepository : IDisposable
    {


        public Task<Character?> Get(int id, CancellationToken cancellationToken, bool tracked = false);
        public Task<IEnumerable<Character>> List(CancellationToken cancellationToken);
        public Task Save(Character character, CancellationToken cancellationToken);
        public Task Delete(int id, CancellationToken cancellationToken);
    }
}
