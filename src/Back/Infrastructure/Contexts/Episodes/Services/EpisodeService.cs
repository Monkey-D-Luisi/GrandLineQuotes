using Application.Common.IoC;
using Application.Contexts.Episodes.Services;
using Domain.Model.Episodes;
using Domain.Model.Episodes.Repositories;
using GrandLineQuotes.Client.Abstractions.DTOs.Episodes;
using Infrastructure.Common.Exceptions;

namespace Infrastructure.Contexts.Episodes.Services
{
    [Ioc(typeof(IEpisodeService))]
    internal class EpisodeService : IEpisodeService
    {


        private readonly IEpisodeRepository repository;


        public EpisodeService(IEpisodeRepository repository)
        {
            this.repository = repository;
        }


        public async Task Save(EpisodeDTO episodeDTO, CancellationToken cancellationToken)
        {
            Episode? episode = await repository.Get(episodeDTO.Number, cancellationToken, tracked: true);

            try
            {
                if (episode is null)
                {
                    episode = Episode.CreateFrom(episodeDTO);

                    await repository.Create(episode, cancellationToken);
                }
                else
                {
                    episode.UpdateFrom(episodeDTO);

                    await repository.Update(episode, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error calling repository", ex);
            }
        }

        public async Task Delete(int number, CancellationToken cancellationToken)
        {
            try
            {
                await repository.Delete(number, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error calling repository", ex);
            }
        }
    }
}
