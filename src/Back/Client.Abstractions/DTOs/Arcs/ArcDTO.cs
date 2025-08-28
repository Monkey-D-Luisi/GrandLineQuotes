using GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using GrandLineQuotes.Client.Abstractions.DTOs.Sagas;

namespace GrandLineQuotes.Client.Abstractions.DTOs.Arcs
{
    public class ArcDTO
    {


        public virtual int? Id { get; set; }
        public virtual FillerType? FillerType { get; set; }
        public virtual ICollection<TranslationDTO>? Titles { get; set; }
        public virtual int? SagaId { get; set; }
        public virtual SagaDTO? Saga { get; set; }
    }
}
