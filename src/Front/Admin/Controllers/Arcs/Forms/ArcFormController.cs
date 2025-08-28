using Admin.Models.Arcs.Forms;
using Admin.Models.Sagas.Forms;
using Application.Contexts.Arcs.Commands;
using Application.Contexts.Arcs.Queries;
using Application.Contexts.Sagas.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Admin.Controllers.Arcs.Forms
{
    public class ArcFormController : Controller
    {
        private readonly IMediator mediator;

        public ArcFormController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("arcs/form")]
        [HttpGet("arcs/form/{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            var model = new ArcViewModel();
            var sagas = await mediator.Send(new ListSagasQuery());
            model.Sagas = sagas.Select(s => new SagaViewModel
            {
                Id = s.Id ?? 0,
                Titles = s.Titles?.ToDictionary(t => t.LanguageCode ?? string.Empty, t => t.Value ?? string.Empty) ?? new()
            });

            if (id.HasValue)
            {
                var arcs = await mediator.Send(new ListArcsQuery());
                var arc = arcs.FirstOrDefault(a => a.Id == id);
                if (arc != null)
                {
                    model.Id = arc.Id ?? 0;
                    model.Titles = arc.Titles?.ToDictionary(t => t.LanguageCode ?? string.Empty, t => t.Value ?? string.Empty) ?? new();
                    model.FillerType = arc.FillerType ?? GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums.FillerType.UNDEFINED;
                    model.SagaId = arc.SagaId ?? 0;
                }
            }
            return PartialView("~/Views/Arcs/Forms/_Arc.cshtml", model);
        }

        [HttpPost("arcs/form")]
        public async Task<IActionResult> Post([FromForm] ArcFormPostModel request)
        {
            try
            {
                var command = new SaveArcCommand(request.Titles, request.FillerType, request.SagaId);
                command.Arc.Id = request.Id;
                await mediator.Send(command);
                return Ok(command.Arc.Id);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
