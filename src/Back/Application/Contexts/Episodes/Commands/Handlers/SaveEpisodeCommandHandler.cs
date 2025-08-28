using Application.Contexts.Episodes.Services;
using MediatR;

namespace Application.Contexts.Episodes.Commands.Handlers
{
    internal class SaveEpisodeCommandHandler : IRequestHandler<SaveEpisodeCommand>
    {


        private readonly IEpisodeService episodeService;


        public SaveEpisodeCommandHandler(IEpisodeService episodeService)
        {
            this.episodeService = episodeService;
        }


        public async Task Handle(SaveEpisodeCommand request, CancellationToken cancellationToken)
        {
            await episodeService.Save(request.Episode, cancellationToken);
        }
    }
}
