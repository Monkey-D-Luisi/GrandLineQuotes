namespace Admin.Models.Episodes.Forms
{
    public class EpisodeFormPostModel
    {
        public int Number { get; set; }
        public Dictionary<string,string> Titles { get; set; } = new();
        public int ArcId { get; set; }
    }
}
