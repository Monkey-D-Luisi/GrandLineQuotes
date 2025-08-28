using Admin.Models.Sagas;
using Application.Contexts.Sagas.Queries;
using Application.Contexts.Sagas.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Admin.Controllers.Sagas
{
    public class SagasController : Controller
    {
        private readonly IMediator mediator;

        public SagasController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var sagas = await mediator.Send(new ListSagasQuery());
            var model = new SagasListViewModel
            {
                Sagas = sagas.Select(s => new SagaListItemViewModel
                {
                    Id = s.Id ?? 0,
                    Title = s.Titles?.FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)?.Value ?? string.Empty
                })
            };
            return View(model);
        }

        [HttpDelete("sagas/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await mediator.Send(new DeleteSagaCommand(id));
            return Ok();
        }
    }
}
