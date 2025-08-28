using Application.Contexts.Quotes.Services;
using GrandLineQuotes.Client.Abstractions.DTOs.Common;
using MediatR;

namespace Application.Contexts.Quotes.Commands.Handlers
{
    internal class DeleteQuoteCommandHandler : IRequestHandler<DeleteQuoteCommand>
    {


        private readonly IQuoteService quoteService;
        private readonly IVideoService videoService;


        public DeleteQuoteCommandHandler(
            IQuoteService service, 
            IVideoService videoService
            )
        {
            this.quoteService = service;
            this.videoService = videoService;
        }


        public async Task Handle(DeleteQuoteCommand request, CancellationToken cancellationToken)
        {
            await quoteService.Delete(request.QuoteId, cancellationToken);
            foreach (var lang in LanguagePath.All)
            {
                var fileName = $"{lang}/{request.QuoteId}.mp4";
                if (await videoService.Exists(fileName, cancellationToken))
                {
                    await videoService.Delete(fileName, cancellationToken);
                }
            }
        }
    }
}
