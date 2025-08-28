using GrandLineQuotes.Client.Abstractions.DTOs.Characters;
using MediatR;

namespace Application.Contexts.Characters.Queries
{
    public class ListCharactersQuery : IRequest<IEnumerable<CharacterDTO>>
    {
    }
}
