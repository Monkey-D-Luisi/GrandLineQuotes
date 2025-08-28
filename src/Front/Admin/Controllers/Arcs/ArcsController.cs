using Admin.Models.Arcs;
using Application.Contexts.Arcs.Queries;
using Application.Contexts.Arcs.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Admin.Controllers.Arcs
{
    public class ArcsController : Controller
    {
        private readonly IMediator mediator;

        public ArcsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var arcs = await mediator.Send(new ListArcsQuery());
            var model = new ArcsListViewModel
            {
                Arcs = arcs.Select(a => new ArcListItemViewModel
                {
                    Id = a.Id ?? 0,
                    Title = a.Titles?.FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)?.Value ?? string.Empty,
                    SagaTitle = a.Saga?.Titles?.FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)?.Value ?? string.Empty,
                    FillerType = a.FillerType ?? GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums.FillerType.UNDEFINED
                })
            };
            return View(model);
        }

        [HttpDelete("arcs/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await mediator.Send(new DeleteArcCommand(id));
            return Ok();
        }
    }
}
