using GrandLineQuotes.Client.Abstractions.DTOs.Episodes;

namespace Application.Contexts.Episodes.Services
{
    public interface IEpisodeService
    {
        public Task Save(EpisodeDTO episode, CancellationToken cancellationToken);
        public Task Delete(int number, CancellationToken cancellationToken);
    }
}
