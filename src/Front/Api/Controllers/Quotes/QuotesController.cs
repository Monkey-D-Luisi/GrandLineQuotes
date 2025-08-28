using Application.Contexts.Quotes.Queries;
using GrandLineQuotes.Client.Abstractions.RequestModels.Quotes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Quotes
{
    public class QuotesController : Controller
    {


        private readonly IMediator mediator;


        public QuotesController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpGet("v1/quotes")]
        public async Task<IActionResult> ListQuotes(QuotesListRequestModel requestModel)
        {
            var query = new ListQuotesQuery(
                requestModel.AuthorId,
                requestModel.ArcId,
                requestModel.SearchTerm
                );
            return Ok(await mediator.Send(query));
        }


        [HttpGet("v1/quotes/{quoteId}")]
        public async Task<IActionResult> GetQuote([FromRoute] int quoteId)
        {
            var query = new GetQuoteQuery(quoteId);
            return Ok(await mediator.Send(query));
        }
    }
}
