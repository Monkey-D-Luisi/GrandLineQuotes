namespace GrandLineQuotes.Client.Abstractions.RequestModels.Quotes
{
    public class QuotesListRequestModel
    {


        public int? AuthorId { get; set; }
        public int? ArcId { get; set; }
        public string? SearchTerm { get; set; }
    }
}
