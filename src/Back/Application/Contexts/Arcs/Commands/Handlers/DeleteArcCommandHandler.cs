using Application.Contexts.Arcs.Services;
using MediatR;

namespace Application.Contexts.Arcs.Commands.Handlers
{
    internal class DeleteArcCommandHandler : IRequestHandler<DeleteArcCommand>
    {
        private readonly IArcService arcService;

        public DeleteArcCommandHandler(IArcService arcService)
        {
            this.arcService = arcService;
        }

        public async Task Handle(DeleteArcCommand request, CancellationToken cancellationToken)
        {
            await arcService.Delete(request.ArcId, cancellationToken);
        }
    }
}
