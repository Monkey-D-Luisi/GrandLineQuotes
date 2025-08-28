using Application.Contexts.Episodes.Services;
using MediatR;

namespace Application.Contexts.Episodes.Commands.Handlers
{
    internal class DeleteEpisodeCommandHandler : IRequestHandler<DeleteEpisodeCommand>
    {
        private readonly IEpisodeService episodeService;

        public DeleteEpisodeCommandHandler(IEpisodeService episodeService)
        {
            this.episodeService = episodeService;
        }

        public async Task Handle(DeleteEpisodeCommand request, CancellationToken cancellationToken)
        {
            await episodeService.Delete(request.Number, cancellationToken);
        }
    }
}
