using Admin.Models.Characters.Forms;
using Application.Contexts.Characters.Commands;
using Application.Contexts.Characters.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers.Characters.Forms
{
    public class CharacterFormController : Controller
    {
        private readonly IMediator mediator;

        public CharacterFormController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("characters/form")]
        [HttpGet("characters/form/{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            var model = new CharacterViewModel();
            if (id.HasValue)
            {
                var characters = await mediator.Send(new ListCharactersQuery());
                var character = characters.FirstOrDefault(c => c.Id == id);
                if (character != null)
                {
                    model.Id = character.Id ?? 0;
                    model.Name = character.Name ?? string.Empty;
                    model.Alias = character.Alias ?? string.Empty;
                }
            }
            return PartialView("~/Views/Characters/Forms/_Character.cshtml", model);
        }

        [HttpPost("characters/form")]
        public async Task<IActionResult> Post([FromForm] CharacterFormPostModel request)
        {
            try
            {
                var command = new SaveCharacterCommand(request.Name, request.Alias);
                command.Character.Id = request.Id;
            await mediator.Send(command);
                return Ok(command.Character.Id);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
