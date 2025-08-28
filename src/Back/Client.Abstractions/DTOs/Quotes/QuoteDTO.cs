using GrandLineQuotes.Client.Abstractions.DTOs.Characters;
using GrandLineQuotes.Client.Abstractions.DTOs.Episodes;

namespace GrandLineQuotes.Client.Abstractions.DTOs.Quotes
{
    public class QuoteDTO
    {


        public virtual int? Id { get; set; }
        public virtual string? OriginalText { get; set; }
        public virtual string? Text { get; set; }
        public virtual ICollection<TranslationDTO>? Translations { get; set; }
        public virtual int? AuthorId { get; set; }
        public virtual CharacterDTO? Author { get; set; }
        public virtual int? EpisodeNumber { get; set; }
        public virtual EpisodeDTO? Episode { get; set; }
        public virtual IDictionary<string, VideoDTO>? Videos { get; set; }
        public virtual bool? IsReviewed { get; set; }
    }
}
