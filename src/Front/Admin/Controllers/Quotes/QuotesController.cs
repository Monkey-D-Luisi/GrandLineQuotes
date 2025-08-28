using Admin.Models.Quotes;
using Admin.Models.Quotes.Forms;
using Application.Contexts.Quotes.Commands;
using Application.Contexts.Quotes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using QuotesListRequestModel = Admin.Models.Quotes.QuotesListRequestModel;

namespace Admin.Controllers.Quotes
{
    public class QuotesController : Controller
    {


        private readonly IMediator mediator;


        public QuotesController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        public async Task<IActionResult> Index(QuotesListRequestModel requestModel)
        {
            var query = new ListQuotesQuery(
                requestModel.AuthorId,
                requestModel.ArcId,
                requestModel.SearchTerm,
                requestModel.IsReviewed
                );
            var quotes = await mediator.Send(query);

            var model = new QuotesListViewModel
            {
                Quotes = quotes.Select(quote => new QuoteListItemViewModel
                {
                    Id = quote?.Id ?? 0,
                    Text = quote?.Text ?? string.Empty,
                    AuthorName = quote?.Author is null 
                        ? string.Empty 
                        : $"{quote.Author.Name}{(string.IsNullOrEmpty(quote.Author.Alias) 
                            ? string.Empty 
                            : $" ({quote.Author.Alias})")}",
                    EpisodeNumber = quote?.EpisodeNumber ?? 0,
                    IsReviewed = quote?.IsReviewed ?? false,
                }),
                Authors = quotes.Select(quote => new AuthorViewModel
                {
                    Id = quote.AuthorId ?? 0,
                    Name = quote.Author is null ? string.Empty : $"{quote.Author.Name}({quote.Author.Alias})"
                }),
                Arcs = quotes.Select(quote => new ArcViewModel
                {
                    Id = quote.Episode?.ArcId ?? 0,
                    Title = quote.Episode?.Arc?.Titles.FirstOrDefault(title => title.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)?.Value ?? string.Empty
                })
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_List", model);
            }

            return View(model);
        }


        [HttpDelete("quotes/{quoteId}")]
        public async Task<IActionResult> Delete([FromRoute] int quoteId)
        {
            var command = new DeleteQuoteCommand(quoteId);
            await mediator.Send(command);

            return Ok();
        }
    }
}
