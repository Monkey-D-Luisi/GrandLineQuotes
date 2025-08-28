using GrandLineQuotes.Client.Abstractions.DTOs.Sagas;

namespace Application.Contexts.Sagas.Services
{
    public interface ISagaService
    {


        public Task Save(SagaDTO saga, CancellationToken cancellationToken);
        public Task Delete(int sagaId, CancellationToken cancellationToken);
    }
}
