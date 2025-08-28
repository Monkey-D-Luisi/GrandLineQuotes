namespace Admin.Models.Characters
{
    public class CharactersListViewModel
    {
        public IEnumerable<CharacterListItemViewModel> Characters { get; set; } = Enumerable.Empty<CharacterListItemViewModel>();
    }
}
