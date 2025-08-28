using Application.Contexts.Characters.Services;
using MediatR;

namespace Application.Contexts.Characters.Commands.Handlers
{
    internal class SaveCharacterCommandHandler : IRequestHandler<SaveCharacterCommand>
    {


        private readonly ICharacterService characterService;


        public SaveCharacterCommandHandler(ICharacterService characterService)
        {
            this.characterService = characterService;
        }


        public async Task Handle(SaveCharacterCommand request, CancellationToken cancellationToken)
        { 
            await characterService.Save(request.Character, cancellationToken);
        }
    }
}
