using Admin.Models.Quotes.Forms;
using Application.Contexts.Arcs.Commands;
using Application.Contexts.Arcs.Queries;
using Application.Contexts.Characters.Commands;
using Application.Contexts.Characters.Queries;
using Application.Contexts.Episodes.Commands;
using Application.Contexts.Episodes.Queries;
using Application.Contexts.Quotes.Commands;
using Application.Contexts.Quotes.Queries;
using Application.Contexts.Sagas.Commands;
using Application.Contexts.Sagas.Queries;
using GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums;
using GrandLineQuotes.Client.Abstractions.DTOs.Common;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Admin.Controllers.Quotes.Forms
{
    public class QuoteFormController : Controller
    {

        /// <summary>Configuration key for the public Minio endpoint.</summary>
        private const string MinioPublicEndpointKey = "MinioPublic:Endpoint";

        /// <summary>Configuration key indicating whether public Minio connections use HTTPS.</summary>
        private const string MinioPublicSecureKey = "MinioPublic:Secure";

        /// <summary>Configuration key for the public Minio bucket that stores quote videos.</summary>
        private const string MinioPublicBucketKey = "MinioPublic:Bucket";

        /// <summary>Configuration key for the path inside the public Minio bucket where quote videos are stored.</summary>
        private const string MinioPublicQuoteVideoPathKey = "MinioPublic:QuoteVideoPath";

        private readonly IMediator mediator;
        private readonly IConfiguration configuration;

        public QuoteFormController(IMediator mediator, IConfiguration configuration)
        {
            this.mediator = mediator;
            this.configuration = configuration;
        }


        [HttpGet]
        [Route("quotes/form")]
        [Route("quotes/form/{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            var model = new QuoteViewModel
            {
                Translations = new Dictionary<string, string>(),
            };

            if (id.HasValue)
            {
                var query = new GetQuoteQuery(id.Value);
                var quote = await mediator.Send(query);

                model.Id = quote?.Id ?? 0;
                model.OriginalText = quote?.OriginalText ?? string.Empty;
                model.Text = quote?.Text ?? string.Empty;
                model.Translations = quote?.Translations?
                    .Where(translation => !string.IsNullOrWhiteSpace(translation?.LanguageCode))
                    .GroupBy(translation => translation!.LanguageCode!)
                    .ToDictionary(group => group.Key, group => group.First().Value ?? string.Empty)
                    ?? new Dictionary<string, string>();
                model.AuthorId = quote?.Author?.Id ?? 0;
                model.EpisodeNumber = quote?.Episode?.Number ?? 0;
                model.IsReviewed = quote?.IsReviewed ?? false;
                model.ArcId = quote?.Episode?.Arc?.Id ?? 0;
                model.ArcFillerType = quote?.Episode?.Arc?.FillerType ?? FillerType.UNDEFINED;
                model.SagaId = quote?.Episode?.Arc?.Saga?.Id ?? 0;
            }

            var authorsQuery = new ListCharactersQuery();
            var authors = await mediator.Send(authorsQuery);
            model.Authors = authors.Select(author => new AuthorViewModel
            {
                Id = author?.Id ?? 0,
                Name = author is null 
                    ? string.Empty 
                    : $"{author.Name}{(string.IsNullOrEmpty(author.Alias) 
                        ? string.Empty 
                        : $" ({author.Alias})")}"
            });

            var episodesQuery = new ListEpisodesQuery();
            var episodes = await mediator.Send(episodesQuery);
            model.Episodes = episodes.Select(episode => new EpisodeViewModel
            {
                Number = episode?.Number ?? 0,
                Title = episode?.Titles?.FirstOrDefault(title => title.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)?.Value ?? string.Empty,
            });

            var arcsQuery = new ListArcsQuery();
            var arcs = await mediator.Send(arcsQuery);
            model.Arcs = arcs.Select(arc => new ArcViewModel
            {
                Id = arc?.Id ?? 0,
                Title = arc?.Titles?.FirstOrDefault(title => title.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)?.Value ?? string.Empty,
            });

            var sagasQuery = new ListSagasQuery();
            var sagas = await mediator.Send(sagasQuery);
            model.Sagas = sagas.Select(saga => new SagaViewModel
            {
                Id = saga?.Id ?? 0,
                Title = saga?.Titles?.FirstOrDefault(title => title.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)?.Value ?? string.Empty
            });

            if (model.Id > 0)
            {
                var endpoint = configuration.GetValue<string>(MinioPublicEndpointKey);
                var secure = configuration.GetValue<bool>(MinioPublicSecureKey);
                var bucket = configuration.GetValue<string>(MinioPublicBucketKey);
                var path = configuration.GetValue<string>(MinioPublicQuoteVideoPathKey);

                if (!string.IsNullOrWhiteSpace(endpoint) &&
                    !string.IsNullOrWhiteSpace(bucket) &&
                    !string.IsNullOrWhiteSpace(path))
                {
                    var scheme = secure ? "https" : "http";
                    model.VideoUrl = $"{scheme}://{endpoint}/{bucket}/{path}/{LanguagePath.English}/{model.Id}.mp4?_={DateTime.UtcNow.Ticks}";
                    model.VideoUrlEs = $"{scheme}://{endpoint}/{bucket}/{path}/{LanguagePath.Spanish}/{model.Id}.mp4?_={DateTime.UtcNow.Ticks}";
                }
            }

            return PartialView("~/Views/Quotes/Forms/_Quote.cshtml", model);
        }


        [HttpPost]
        [Route("quotes/form")]
        public async Task<IActionResult> Post([FromForm] QuoteFormPostRequestModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                var errs = ModelState.Values.SelectMany(v => v.Errors)
                                            .Select(e => e.Exception?.Message ?? e.ErrorMessage);
                return BadRequest(new { errors = errs });
            }


            if (requestModel.SagaId == -1)
            {
                var saveNewSagaCommand = new SaveSagaCommand(requestModel.NewSagaTitles?.ToDictionary() ?? new Dictionary<string, string>());

                await mediator.Send(saveNewSagaCommand);

                requestModel.SagaId = saveNewSagaCommand.Saga.Id;
            }

            if (requestModel.ArcId == -1)
            {
                var saveNewArcCommand = new SaveArcCommand(
                    requestModel.NewArcTitles?.ToDictionary() ?? new Dictionary<string, string>(), 
                    (FillerType)Enum.ToObject(typeof(FillerType), 
                    requestModel.NewArcFillerTypeId ?? 0), 
                    requestModel.SagaId ?? 0);

                await mediator.Send(saveNewArcCommand);

                requestModel.ArcId = saveNewArcCommand.Arc.Id;
            }

            if (requestModel.AuthorId == -1)
            {
                var saveNewAuthorCommand = new SaveCharacterCommand(requestModel.NewAuthorName ?? string.Empty);

                await mediator.Send(saveNewAuthorCommand);

                requestModel.AuthorId = saveNewAuthorCommand.Character.Id;
            }

            if (requestModel.EpisodeNumber == -1)
            {
                var saveNewEpisodeCommand = new SaveEpisodeCommand
                (
                    requestModel.NewEpisodeNumber ?? 0,
                    requestModel.NewEpisodeTitles?.ToDictionary() ?? new Dictionary<string, string>(),
                    requestModel.ArcId ?? 0);

                await mediator.Send(saveNewEpisodeCommand);

                requestModel.EpisodeNumber = saveNewEpisodeCommand.Episode.Number;
            }

            var videos = new Dictionary<string, VideoDTO>();
            if (requestModel.VideoFile != null)
            {
                videos[LanguagePath.English] = new VideoDTO
                {
                    Content = requestModel.VideoFile.OpenReadStream(),
                    ContentType = requestModel.VideoFile.ContentType
                };
            }
            if (requestModel.VideoFileEs != null)
            {
                videos[LanguagePath.Spanish] = new VideoDTO
                {
                    Content = requestModel.VideoFileEs.OpenReadStream(),
                    ContentType = requestModel.VideoFileEs.ContentType
                };
            }

            var saveQuoteCommand = new SaveQuoteCommand
            (
                requestModel.Id ?? 0,
                requestModel.OriginalText ?? string.Empty,
                requestModel.Text ?? string.Empty,
                requestModel.Translations?.ToDictionary() ?? new Dictionary<string, string>(),
                requestModel.AuthorId ?? 0,
                requestModel.EpisodeNumber ?? 0,
                requestModel.IsReviewed ?? false,
                videos
            );
            await mediator.Send(saveQuoteCommand);

            return Ok(saveQuoteCommand.Quote.Id);
        }
    }
}
