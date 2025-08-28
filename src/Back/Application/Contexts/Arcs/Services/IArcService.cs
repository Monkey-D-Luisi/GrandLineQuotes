using GrandLineQuotes.Client.Abstractions.DTOs.Arcs;

namespace Application.Contexts.Arcs.Services
{
    public interface IArcService
    {


        public Task Save(ArcDTO arc, CancellationToken cancellationToken);
        public Task Delete(int arcId, CancellationToken cancellationToken);
    }
}
