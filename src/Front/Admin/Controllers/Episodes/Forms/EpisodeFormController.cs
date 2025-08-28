using Admin.Models.Arcs.Forms;
using Admin.Models.Episodes.Forms;
using Application.Contexts.Arcs.Queries;
using Application.Contexts.Episodes.Commands;
using Application.Contexts.Episodes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Admin.Controllers.Episodes.Forms
{
    public class EpisodeFormController : Controller
    {
        private readonly IMediator mediator;

        public EpisodeFormController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("episodes/form")]
        [HttpGet("episodes/form/{number}")]
        public async Task<IActionResult> Get(int? number)
        {
            var arcs = await mediator.Send(new ListArcsQuery());
            var model = new EpisodeViewModel
            {
                Arcs = arcs.Select(a => new ArcViewModel
                {
                    Id = a.Id ?? 0,
                    Titles = a.Titles?.ToDictionary(t => t.LanguageCode ?? string.Empty, t => t.Value ?? string.Empty) ?? new(),
                    FillerType = a.FillerType ?? GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums.FillerType.UNDEFINED,
                    SagaId = a.SagaId ?? 0
                })
            };
            if (number.HasValue)
            {
                var episodes = await mediator.Send(new ListEpisodesQuery());
                var episode = episodes.FirstOrDefault(e => e.Number == number);
                if (episode != null)
                {
                    model.Number = episode.Number;
                    model.Titles = episode.Titles?.ToDictionary(t => t.LanguageCode ?? string.Empty, t => t.Value ?? string.Empty) ?? new();
                    model.ArcId = episode.ArcId ?? 0;
                }
            }
            return PartialView("~/Views/Episodes/Forms/_Episode.cshtml", model);
        }

        [HttpPost("episodes/form")]
        public async Task<IActionResult> Post([FromForm] EpisodeFormPostModel request)
        {
            try
            {
                var command = new SaveEpisodeCommand(request.Number, request.Titles, request.ArcId);
                await mediator.Send(command);
                return Ok(command.Episode.Number);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
