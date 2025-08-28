using GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums;

namespace Admin.Models.Arcs
{
    public class ArcListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SagaTitle { get; set; } = string.Empty;
        public FillerType FillerType { get; set; }
    }
}
