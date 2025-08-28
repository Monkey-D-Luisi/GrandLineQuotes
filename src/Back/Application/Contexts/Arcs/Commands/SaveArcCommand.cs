
using Dawn;
using GrandLineQuotes.Client.Abstractions.DTOs.Arcs;
using GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using MediatR;

namespace Application.Contexts.Arcs.Commands
{
    public class SaveArcCommand : IRequest
    {


        public SaveArcCommand(IDictionary<string, string> titles, FillerType fillerType, int sagaId)
        {
            Guard.Argument(titles, nameof(titles)).NotEmpty();
            Guard.Argument(fillerType, nameof(fillerType)).NotDefault();
            Guard.Argument(sagaId, nameof(sagaId)).GreaterThan(0);

            Arc = new ArcDTO
            {
                Titles = titles
                    .Select(title => new TranslationDTO { LanguageCode = title.Key, Value = title.Value })
                    .ToList(),
                FillerType = fillerType,
                SagaId = sagaId
            };
        }


        public ArcDTO Arc { get; }
    }
}
