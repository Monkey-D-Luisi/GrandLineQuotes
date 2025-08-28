using Application.Contexts.Sagas.Services;
using MediatR;

namespace Application.Contexts.Sagas.Commands.Handlers
{
    internal class SaveSagaCommandHandler : IRequestHandler<SaveSagaCommand>
    {


        private readonly ISagaService sagaService;


        public SaveSagaCommandHandler(ISagaService sagaService)
        {
            this.sagaService = sagaService;
        }


        public async Task Handle(SaveSagaCommand request, CancellationToken cancellationToken)
        {
            await sagaService.Save(request.Saga, cancellationToken);
        }
    }
}
