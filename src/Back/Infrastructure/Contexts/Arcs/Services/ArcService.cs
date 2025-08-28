using Application.Common.IoC;
using Application.Contexts.Arcs.Services;
using Domain.Model.Arcs;
using Domain.Model.Arcs.Repositories;
using GrandLineQuotes.Client.Abstractions.DTOs.Arcs;
using Infrastructure.Common.Exceptions;

namespace Infrastructure.Contexts.Arcs.Services
{
    [Ioc(typeof(IArcService))]
    internal class ArcService : IArcService
    {


        private readonly IArcRepository repository;


        public ArcService(IArcRepository arcRepository)
        {
            this.repository = arcRepository;
        }


        public async Task Save(ArcDTO arcDTO, CancellationToken cancellationToken)
        {
            Arc? arc = null;

            try
            {
                if ((arcDTO.Id ?? 0) > 0)
                    arc = await repository.Get(arcDTO.Id ?? 0, cancellationToken, tracked: true);

                if (arc is null)
                    arc = Arc.CreateFrom(arcDTO);
                else
                    arc.UpdateFrom(arcDTO);

                await repository.Save(arc, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error calling repository", ex);
            }

            arcDTO.Id = arc.Id;
        }

        public async Task Delete(int arcId, CancellationToken cancellationToken)
        {
            try
            {
                await repository.Delete(arcId, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error calling repository", ex);
            }
        }
    }
}
