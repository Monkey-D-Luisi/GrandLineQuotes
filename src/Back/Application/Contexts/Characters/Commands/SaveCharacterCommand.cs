using Dawn;
using GrandLineQuotes.Client.Abstractions.DTOs.Characters;
using MediatR;

namespace Application.Contexts.Characters.Commands
{
    public class SaveCharacterCommand : IRequest
    {


        public SaveCharacterCommand(string name, string? alias = null)
        {
            Guard.Argument(name, nameof(name)).NotWhiteSpace();

            Character = new CharacterDTO
            {
                Name = name,
                Alias = alias
            };
        }


        public CharacterDTO Character { get; }
    }
}
