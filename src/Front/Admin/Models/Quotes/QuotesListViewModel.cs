using Admin.Models.Quotes.Forms;

namespace Admin.Models.Quotes
{
    public class QuotesListViewModel
    {


        public IEnumerable<QuoteListItemViewModel> Quotes { get; internal set; }
        public IEnumerable<AuthorViewModel> Authors { get; internal set; }
        public IEnumerable<ArcViewModel> Arcs { get; internal set; }
    }
}
