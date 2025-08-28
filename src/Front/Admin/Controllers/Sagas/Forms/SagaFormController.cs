using Admin.Models.Sagas.Forms;
using Application.Contexts.Sagas.Commands;
using Application.Contexts.Sagas.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Admin.Controllers.Sagas.Forms
{
    public class SagaFormController : Controller
    {
        private readonly IMediator mediator;

        public SagaFormController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("sagas/form")]
        [HttpGet("sagas/form/{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            var model = new SagaViewModel();
            if (id.HasValue)
            {
                var sagas = await mediator.Send(new ListSagasQuery());
                var saga = sagas.FirstOrDefault(s => s.Id == id);
                if (saga != null)
                {
                    model.Id = saga.Id ?? 0;
                    model.Titles = saga.Titles?.ToDictionary(t => t.LanguageCode ?? string.Empty, t => t.Value ?? string.Empty) ?? new();
                }
            }
            return PartialView("~/Views/Sagas/Forms/_Saga.cshtml", model);
        }

        [HttpPost("sagas/form")]
        public async Task<IActionResult> Post([FromForm] SagaFormPostModel request)
        {
            try
            {
                var command = new SaveSagaCommand(request.Titles);
                command.Saga.Id = request.Id;
                await mediator.Send(command);
                return Ok(command.Saga.Id);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
