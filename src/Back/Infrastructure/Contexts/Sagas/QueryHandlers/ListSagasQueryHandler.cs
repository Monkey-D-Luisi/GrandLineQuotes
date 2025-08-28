using Application.Contexts.Sagas.Queries;
using Domain.Model.Sagas.Repositories;
using GrandLineQuotes.Client.Abstractions.DTOs.Sagas;
using MediatR;

namespace Infrastructure.Contexts.Sagas.QueryHandlers
{
    internal class ListSagasQueryHandler : IRequestHandler<ListSagasQuery, IEnumerable<SagaDTO>>
    {


        private readonly ISagaRepository sagaRepository;


        public ListSagasQueryHandler(ISagaRepository sagaRepository)
        {
            this.sagaRepository = sagaRepository;
        }


        public async Task<IEnumerable<SagaDTO>> Handle(ListSagasQuery request, CancellationToken cancellationToken)
        {
            var sagas = await sagaRepository.List(cancellationToken);

            return sagas.Select(saga => saga.GetSnapshot()) ?? new List<SagaDTO>();
        }
    }
}
