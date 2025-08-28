using Application.Common.IoC;
using Application.Contexts.Characters.Services;
using Domain.Model.Characters;
using Domain.Model.Characters.Abstractions;
using GrandLineQuotes.Client.Abstractions.DTOs.Characters;
using Infrastructure.Common.Exceptions;

namespace Infrastructure.Contexts.Characters.Services
{
    [Ioc(typeof(ICharacterService))]
    internal class CharacterService : ICharacterService
    {


        private readonly ICharacterRepository repository;


        public CharacterService(ICharacterRepository repository)
        {
            this.repository = repository;
        }


        public async Task Save(CharacterDTO characterDTO, CancellationToken cancellationToken)
        {
            Character? character = null;

            try
            {
                if ((characterDTO.Id ?? 0) > 0)
                    character = await repository.Get(characterDTO.Id ?? 0, cancellationToken, tracked: true);

                if (character is null)
                    character = Character.CreateFrom(characterDTO);
                else
                    character.UpdateFrom(characterDTO);

                await repository.Save(character, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error calling repository", ex);
            }

            characterDTO.Id = character.Id;
        }

        public async Task Delete(int characterId, CancellationToken cancellationToken)
        {
            try
            {
                await repository.Delete(characterId, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error calling repository", ex);
            }
        }
    }
}
