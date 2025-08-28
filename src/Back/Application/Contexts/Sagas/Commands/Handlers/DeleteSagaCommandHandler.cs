using Application.Contexts.Sagas.Services;
using MediatR;

namespace Application.Contexts.Sagas.Commands.Handlers
{
    internal class DeleteSagaCommandHandler : IRequestHandler<DeleteSagaCommand>
    {
        private readonly ISagaService sagaService;

        public DeleteSagaCommandHandler(ISagaService sagaService)
        {
            this.sagaService = sagaService;
        }

        public async Task Handle(DeleteSagaCommand request, CancellationToken cancellationToken)
        {
            await sagaService.Delete(request.SagaId, cancellationToken);
        }
    }
}
