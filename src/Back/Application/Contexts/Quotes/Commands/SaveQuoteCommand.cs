using Dawn;
using Domain.Model.Quotes;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace Application.Contexts.Quotes.Commands
{
    public class SaveQuoteCommand : IRequest
    {


        public SaveQuoteCommand
        (
            int id,
            string originalText,
            string text,
            IDictionary<string, string> translations,
            int authorId,
            int episodeNumber,
            bool isReviewed,
            IDictionary<string, VideoDTO> videos)
        {
            Guard.Argument(originalText, nameof(originalText)).NotWhiteSpace();
            Guard.Argument(text, nameof(text)).NotWhiteSpace();
            Guard.Argument(translations, nameof(translations)).NotEmpty();
            Guard.Argument(authorId, nameof(authorId)).Positive();
            Guard.Argument(episodeNumber, nameof(episodeNumber)).Positive();


            Quote = new QuoteDTO
            {
                Id = id,
                OriginalText = originalText,
                Text = text,
                Translations = translations
                    .Select(translation => new TranslationDTO { LanguageCode = translation.Key, Value = translation.Value })
                    .ToList(),
                AuthorId = authorId,
                EpisodeNumber = episodeNumber,
                IsReviewed = isReviewed,
                Videos = videos
            };

            if (id > 0 && Quote.Videos is not null)
            {
                foreach (var video in Quote.Videos)
                {
                    video.Value.Name = $"{video.Key}/{id}.mp4";
                }
            }
        }


        public QuoteDTO Quote { get; }
    }
}
