namespace Admin.Models.Characters.Forms
{
    public class CharacterFormPostModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
    }
}
