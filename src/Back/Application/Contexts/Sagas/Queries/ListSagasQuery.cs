using GrandLineQuotes.Client.Abstractions.DTOs.Sagas;
using MediatR;

namespace Application.Contexts.Sagas.Queries
{
    public class ListSagasQuery : IRequest<IEnumerable<SagaDTO>>
    {
    }
}
