using Application.Contexts.Characters.Services;
using MediatR;

namespace Application.Contexts.Characters.Commands.Handlers
{
    internal class DeleteCharacterCommandHandler : IRequestHandler<DeleteCharacterCommand>
    {
        private readonly ICharacterService characterService;

        public DeleteCharacterCommandHandler(ICharacterService characterService)
        {
            this.characterService = characterService;
        }

        public async Task Handle(DeleteCharacterCommand request, CancellationToken cancellationToken)
        {
            await characterService.Delete(request.CharacterId, cancellationToken);
        }
    }
}
