using MediatR;

namespace Application.Contexts.Characters.Commands
{
    public record DeleteCharacterCommand(int CharacterId) : IRequest;
}
