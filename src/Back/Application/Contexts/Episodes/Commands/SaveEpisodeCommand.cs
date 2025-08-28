using Dawn;
using GrandLineQuotes.Client.Abstractions.DTOs.Episodes;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using MediatR;

namespace Application.Contexts.Episodes.Commands
{
    public class SaveEpisodeCommand : IRequest
    {


        public SaveEpisodeCommand(
            int number, 
            IDictionary<string, string> titles, 
            int arcId)
        {
            Guard.Argument(number, nameof(number)).Positive();
            Guard.Argument(titles, nameof(titles)).NotEmpty();
            Guard.Argument(arcId, nameof(arcId)).GreaterThan(0);

            Episode = new EpisodeDTO
            {
                Number = number,
                Titles = titles
                    .Select(title => new TranslationDTO { LanguageCode = title.Key, Value = title.Value })
                    .ToList(),
                ArcId = arcId
            };
        }


        public EpisodeDTO Episode { get; }
    }
}
