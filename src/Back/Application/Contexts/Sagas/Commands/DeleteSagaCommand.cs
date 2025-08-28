using MediatR;

namespace Application.Contexts.Sagas.Commands
{
    public record DeleteSagaCommand(int SagaId) : IRequest;
}
