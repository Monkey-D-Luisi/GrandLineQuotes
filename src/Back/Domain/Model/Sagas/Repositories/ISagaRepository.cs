
namespace Domain.Model.Sagas.Repositories
{
    public interface ISagaRepository : IDisposable
    {


        public Task<Saga?> Get(int? id, CancellationToken cancellationToken, bool tracked = false);
        public Task<IEnumerable<Saga>> List(CancellationToken cancellationToken);
        public Task Save(Saga saga, CancellationToken cancellationToken);
        public Task Delete(int id, CancellationToken cancellationToken);
    }
}
