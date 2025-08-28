namespace Admin.Models.Quotes
{
    public class QuoteListItemViewModel
    {


        public int Id { get; set; }
        public string AuthorName { get; set; }
        public int EpisodeNumber { get; set; }
        public string Text { get; set; }
        public bool IsReviewed { get; set; }
    }
}