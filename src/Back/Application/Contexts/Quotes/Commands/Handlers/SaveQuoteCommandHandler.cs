using Application.Contexts.Quotes.Services;
using Dawn;
using GrandLineQuotes.Client.Abstractions.DTOs.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Contexts.Quotes.Commands.Handlers
{
    internal class SaveQuoteCommandHandler : IRequestHandler<SaveQuoteCommand>
    {
        private readonly IQuoteService quoteService;
        private readonly IVideoService videoService;
        private readonly ILogger<SaveQuoteCommandHandler> logger;

        public SaveQuoteCommandHandler(
            IQuoteService service,
            IVideoService videoService,
            ILogger<SaveQuoteCommandHandler> logger)
        {
            this.quoteService = service;
            this.videoService = videoService;
            this.logger = logger;
        }

        public async Task Handle(SaveQuoteCommand request, CancellationToken cancellationToken)
        {
            var isReviewed = request.Quote.IsReviewed ?? false;
            var allLanguagesHaveVideo = await LanguagesHaveVideoAsync(request, cancellationToken);

            if (isReviewed && !allLanguagesHaveVideo)
            {
                throw new ArgumentException(
                    "Quote can't be marked as reviewed if it doesn't have a related video uploaded in all languages");
            }

            if (request.Quote.Videos != null && (request.Quote.Id ?? 0) > 0)
            {
                foreach (var video in request.Quote.Videos.Values)
                {
                    var fileName = video.Name ?? string.Empty;
                    if (video.Content?.Length > 0 && await videoService.Exists(fileName, cancellationToken))
                    {
                        await videoService.Delete(fileName, cancellationToken);
                    }
                }
            }

            await quoteService.Save(request.Quote, cancellationToken);

            if (request.Quote.Videos != null)
            {
                foreach (var video in request.Quote.Videos.Values)
                {
                    if (video.Content?.Length > 0)
                    {
                        Guard.Argument(video.ContentType, nameof(video.ContentType)).Equal("video/mp4");

                        await videoService.Upload(
                            video.Content,
                            video.Name ?? string.Empty,
                            video.ContentType ?? string.Empty,
                            cancellationToken
                        );
                    }
                }
            }
        }

        private async Task<bool> LanguagesHaveVideoAsync(SaveQuoteCommand request, CancellationToken cancellationToken)
        {
            foreach (var lang in LanguagePath.All)
            {
                var hasValidNewVideo = request.Quote.Videos != null &&
                    request.Quote.Videos.TryGetValue(lang, out var video) &&
                    video.Content != null &&
                    video.Content.Length > 0 &&
                    video.ContentType == "video/mp4";

                if (hasValidNewVideo)
                    continue;

                if (request.Quote.Id > 0)
                {
                    var fileName = $"{lang}/{request.Quote.Id}.mp4";
                    if (await videoService.Exists(fileName, cancellationToken))
                        continue;
                }

                return false;
            }

            return true;
        }
    }
}
