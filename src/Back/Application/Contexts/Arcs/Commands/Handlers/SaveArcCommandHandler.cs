using Application.Contexts.Arcs.Services;
using MediatR;

namespace Application.Contexts.Arcs.Commands.Handlers
{
    internal class SaveArcCommandHandler : IRequestHandler<SaveArcCommand>
    {


        private readonly IArcService arcService;


        public SaveArcCommandHandler(IArcService arcService)
        {
            this.arcService = arcService;
        }


        public async Task Handle(SaveArcCommand request, CancellationToken cancellationToken)
        {
            await arcService.Save(request.Arc, cancellationToken);
        }
    }
}
