namespace Admin.Models.Sagas
{
    public class SagasListViewModel
    {
        public IEnumerable<SagaListItemViewModel> Sagas { get; set; } = Enumerable.Empty<SagaListItemViewModel>();
    }
}
