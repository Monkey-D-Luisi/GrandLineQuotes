using GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums;

namespace Admin.Models.Arcs.Forms
{
    public class ArcFormPostModel
    {
        public int Id { get; set; }
        public Dictionary<string,string> Titles { get; set; } = new();
        public FillerType FillerType { get; set; }
        public int SagaId { get; set; }
    }
}
