namespace Admin.Models.Arcs
{
    public class ArcsListViewModel
    {
        public IEnumerable<ArcListItemViewModel> Arcs { get; set; } = Enumerable.Empty<ArcListItemViewModel>();
    }
}
