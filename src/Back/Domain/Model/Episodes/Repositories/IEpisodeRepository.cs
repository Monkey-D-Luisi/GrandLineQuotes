
namespace Domain.Model.Episodes.Repositories
{
    public interface IEpisodeRepository : IDisposable
    {


        public Task<Episode?> Get(int number, CancellationToken cancellationToken, bool tracked = false);
        public Task<IEnumerable<Episode>> List();
        public Task Create(Episode episode, CancellationToken cancellationToken);
        public Task Update(Episode episode, CancellationToken cancellationToken);
        public Task Delete(int number, CancellationToken cancellationToken);
    }
}
