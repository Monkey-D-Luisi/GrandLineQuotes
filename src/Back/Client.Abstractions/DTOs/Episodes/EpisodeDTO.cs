using GrandLineQuotes.Client.Abstractions.DTOs.Arcs;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;

namespace GrandLineQuotes.Client.Abstractions.DTOs.Episodes
{
    public class EpisodeDTO
    {


        public virtual int Number { get; set; }
        public virtual ICollection<TranslationDTO>? Titles { get; set; }
        public virtual int? ArcId { get; set; }
        public ArcDTO? Arc { get; set; }
    }
}
