namespace Admin.Models.Episodes
{
    public class EpisodeListItemViewModel
    {
        public int Number { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ArcTitle { get; set; } = string.Empty;
        public int? ArcId { get; set; }
    }
}
