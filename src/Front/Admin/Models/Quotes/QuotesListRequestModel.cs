namespace Admin.Models.Quotes
{
    public class QuotesListRequestModel : GrandLineQuotes.Client.Abstractions.RequestModels.Quotes.QuotesListRequestModel
    {


        public bool? IsReviewed { get; set; }
    }
}
