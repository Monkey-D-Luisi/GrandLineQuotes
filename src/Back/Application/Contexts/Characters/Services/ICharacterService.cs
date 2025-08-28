using Domain.Model.Characters;
using GrandLineQuotes.Client.Abstractions.DTOs.Characters;

namespace Application.Contexts.Characters.Services
{
    public interface ICharacterService
    {


        public Task Save(CharacterDTO character, CancellationToken cancellationToken);
        public Task Delete(int characterId, CancellationToken cancellationToken);
    }
}
