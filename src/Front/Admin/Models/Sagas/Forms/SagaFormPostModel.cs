namespace Admin.Models.Sagas.Forms
{
    public class SagaFormPostModel
    {
        public int Id { get; set; }
        public Dictionary<string,string> Titles { get; set; } = new();
    }
}
