using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;

namespace GrandLineQuotes.Client.Abstractions.DTOs.Sagas
{
    public class SagaDTO
    {


        public virtual int? Id { get; set; }
        public virtual ICollection<TranslationDTO>? Titles { get; set; }
    }
}
