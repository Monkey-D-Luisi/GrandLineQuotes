namespace Admin.Models.Sagas.Forms
{
    public class SagaViewModel
    {
        public int Id { get; set; }
        public Dictionary<string,string> Titles { get; set; } = new();
    }
}
