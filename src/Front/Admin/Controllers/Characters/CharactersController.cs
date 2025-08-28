using Admin.Models.Characters;
using Application.Contexts.Characters.Queries;
using Application.Contexts.Characters.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers.Characters
{
    public class CharactersController : Controller
    {
        private readonly IMediator mediator;

        public CharactersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var characters = await mediator.Send(new ListCharactersQuery());
            var model = new CharactersListViewModel
            {
                Characters = characters.Select(c => new CharacterListItemViewModel
                {
                    Id = c.Id ?? 0,
                    Name = c.Name ?? string.Empty,
                    Alias = c.Alias ?? string.Empty
                })
            };
            return View(model);
        }

        [HttpDelete("characters/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await mediator.Send(new DeleteCharacterCommand(id));
            return Ok();
        }
    }
}
