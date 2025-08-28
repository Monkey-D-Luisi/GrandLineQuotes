using Application.Common.IoC;
using Application.Contexts.Sagas.Services;
using Domain.Model.Sagas;
using Domain.Model.Sagas.Repositories;
using GrandLineQuotes.Client.Abstractions.DTOs.Sagas;
using Infrastructure.Common.Exceptions;

namespace Infrastructure.Contexts.Sagas.Services
{
    [Ioc(typeof(ISagaService))]
    internal class SagaService : ISagaService
    {


        private readonly ISagaRepository repository;


        public SagaService(ISagaRepository repository)
        {
            this.repository = repository;
        }


        public async Task Save(SagaDTO sagaDTO, CancellationToken cancellationToken)
        {
            Saga? saga = null;

            try
            {
                if ((sagaDTO.Id ?? 0) > 0)
                    saga = await repository.Get(sagaDTO.Id ?? 0, cancellationToken, tracked: true);

                if (saga is null)
                    saga = Saga.CreateFrom(sagaDTO);
                else
                    saga.UpdateFrom(sagaDTO);

                await repository.Save(saga, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error calling repository", ex);
            }

            sagaDTO.Id = saga.Id;
        }

        public async Task Delete(int sagaId, CancellationToken cancellationToken)
        {
            try
            {
                await repository.Delete(sagaId, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error calling repository", ex);
            }
        }
    }
}
