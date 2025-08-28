using Admin.Models.Arcs.Forms;

namespace Admin.Models.Episodes.Forms
{
    public class EpisodeViewModel
    {
        public int Number { get; set; }
        public Dictionary<string,string> Titles { get; set; } = new();
        public int ArcId { get; set; }
        public IEnumerable<ArcViewModel> Arcs { get; set; } = Enumerable.Empty<ArcViewModel>();
    }
}
