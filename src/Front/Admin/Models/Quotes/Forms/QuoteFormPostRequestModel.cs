namespace Admin.Models.Quotes.Forms
{
    public class QuoteFormPostRequestModel
    {


        public int? Id { get; set; }
        public string? OriginalText { get; set; }
        public string? Text { get; set; }
        public IDictionary<string, string>? Translations { get; set; }
        public int? AuthorId { get; set; }
        public int? EpisodeNumber { get; set; }
        public bool? IsReviewed { get; set; }
        public int? ArcId { get; set; }
        public int? SagaId { get; set; }
        public IFormFile? VideoFile { get; set; }
        public IFormFile? VideoFileEs { get; set; }

        public string? NewAuthorName { get; set; }
        public int? NewEpisodeNumber { get; set; }
        public IDictionary<string, string>? NewEpisodeTitles { get; set; }
        public IDictionary<string, string>? NewArcTitles { get; set; }
        public int? NewArcFillerTypeId { get; set; }
        public IDictionary<string, string>? NewSagaTitles { get; set; }
    }
}
