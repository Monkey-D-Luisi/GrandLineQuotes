using Admin.Models.Arcs.Forms;
using Admin.Models.Episodes;
using Application.Contexts.Arcs.Queries;
using Application.Contexts.Episodes.Queries;
using Application.Contexts.Episodes.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Admin.Controllers.Episodes
{
    public class EpisodesController : Controller
    {


        private readonly IMediator mediator;


        public EpisodesController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        public async Task<IActionResult> Index(int? arcId)
        {
            var arcs = await mediator.Send(new ListArcsQuery());
            var episodes = await mediator.Send(new ListEpisodesQuery());

            if (arcId.HasValue)
            {
                episodes = episodes.Where(e => e.ArcId == arcId.Value);
            }

            var model = new EpisodesListViewModel
            {
                Episodes = episodes.Select(e => new EpisodeListItemViewModel
                {
                    Number = e.Number,
                    Title = e.Titles?.FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)?.Value ?? string.Empty,
                    ArcTitle = e.Arc?.Titles?.FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)?.Value ?? string.Empty,
                    ArcId = e.ArcId
                }),
                Arcs = arcs.Select(a => new ArcViewModel
                {
                    Id = a.Id ?? 0,
                    Titles = a.Titles?.ToDictionary(t => t.LanguageCode ?? string.Empty, t => t.Value ?? string.Empty) ?? new(),
                    FillerType = a.FillerType ?? GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums.FillerType.UNDEFINED,
                    SagaId = a.SagaId ?? 0
                }),
                SelectedArcId = arcId
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_List", model);
            }

            return View(model);
        }


        [HttpDelete("episodes/{number}")]
        public async Task<IActionResult> Delete(int number)
        {
            await mediator.Send(new DeleteEpisodeCommand(number));
            return Ok();
        }
    }
}
