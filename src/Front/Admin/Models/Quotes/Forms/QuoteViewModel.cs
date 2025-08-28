using GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums;

namespace Admin.Models.Quotes.Forms
{
    public class QuoteViewModel
    {


        public int Id { get; set; }
        public string OriginalText { get; internal set; }
        public string Text { get; set; }
        public IDictionary<string, string> Translations { get; internal set; }
        public int AuthorId { get; set; }
        public int EpisodeNumber { get; set; }
        public bool IsReviewed { get; set; }
        public int ArcId { get; set; }
        public FillerType ArcFillerType { get; set; }
        public int SagaId { get; set; }
        public string? VideoUrl { get; set; }
        public string? VideoUrlEs { get; set; }
        public IEnumerable<AuthorViewModel> Authors { get; set; }
        public IEnumerable<EpisodeViewModel> Episodes { get; set; }
        public IEnumerable<ArcViewModel> Arcs { get; set; }
        public IEnumerable<FillerType> FillerTypes
        {
            get
            {
                return Enum.GetValues(typeof(FillerType))
                   .Cast<FillerType>()
                   .ToList();
            }
        }
        public IEnumerable<SagaViewModel> Sagas { get; set; }
    }
}
