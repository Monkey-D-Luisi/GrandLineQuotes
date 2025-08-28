using Admin.Models.Arcs.Forms;

namespace Admin.Models.Episodes
{
    public class EpisodesListViewModel
    {
        public IEnumerable<EpisodeListItemViewModel> Episodes { get; set; } = Enumerable.Empty<EpisodeListItemViewModel>();
        public IEnumerable<ArcViewModel> Arcs { get; set; } = Enumerable.Empty<ArcViewModel>();
        public int? SelectedArcId { get; set; }
    }
}
