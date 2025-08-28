
namespace Domain.Model.Arcs.Repositories
{
    public interface IArcRepository : IDisposable
    {


        public Task<Arc?> Get(int? id, CancellationToken cancellationToken, bool tracked = false);
        public Task<IEnumerable<Arc>> List(CancellationToken cancellationToken);
        public Task Save(Arc arc, CancellationToken cancellationToken);
        public Task Delete(int id, CancellationToken cancellationToken);
    }
}
