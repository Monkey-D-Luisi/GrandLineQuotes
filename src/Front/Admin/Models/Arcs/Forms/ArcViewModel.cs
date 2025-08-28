using GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums;
using Admin.Models.Sagas.Forms;

namespace Admin.Models.Arcs.Forms
{
    public class ArcViewModel
    {
        public int Id { get; set; }
        public Dictionary<string,string> Titles { get; set; } = new();
        public FillerType FillerType { get; set; }
        public int SagaId { get; set; }
        public IEnumerable<SagaViewModel> Sagas { get; set; } = Enumerable.Empty<SagaViewModel>();
        public IEnumerable<FillerType> FillerTypes { get; set; } = Enum.GetValues<FillerType>();
    }
}
