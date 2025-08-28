using Dawn;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using GrandLineQuotes.Client.Abstractions.DTOs.Sagas;
using MediatR;

namespace Application.Contexts.Sagas.Commands
{
    public class SaveSagaCommand : IRequest
    {


        public SaveSagaCommand(IDictionary<string, string> titles)
        {
            Guard.Argument(titles, nameof(titles)).NotEmpty();

            Saga = new SagaDTO
            {
                Titles = titles
                    .Select(title => new TranslationDTO { LanguageCode = title.Key, Value = title.Value })
                    .ToList(),
            };
        }


        public SagaDTO Saga { get; }
    }
}
