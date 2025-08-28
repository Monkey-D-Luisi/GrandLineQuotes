using Admin.Controllers.Quotes.Forms;
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
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums;
using GrandLineQuotes.Client.Abstractions.DTOs.Arcs;
using GrandLineQuotes.Client.Abstractions.DTOs.Characters;
using GrandLineQuotes.Client.Abstractions.DTOs.Episodes;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using GrandLineQuotes.Client.Abstractions.DTOs.Sagas;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Admin.Tests.Unit.Controllers.Quotes.Forms
{
    [TestFixture(Category = "Unit")]
    internal class QuoteFormControllerShould
    {


        [Test]
        public async Task Get_ShouldIgnoreInvalidTranslationLanguageCodes()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var configurationMock = new Mock<IConfiguration>();
            var sectionMock = new Mock<IConfigurationSection>();
            configurationMock.Setup(c => c.GetSection(It.IsAny<string>())).Returns(sectionMock.Object);

            var controller = new QuoteFormController(mediatorMock.Object, configurationMock.Object);

            var quote = new QuoteDTO
            {
                Id = 1,
                Translations = new List<TranslationDTO>
                {
                    new TranslationDTO { LanguageCode = null, Value = "null" },
                    new TranslationDTO { LanguageCode = string.Empty, Value = "empty" },
                    new TranslationDTO { LanguageCode = "en", Value = "Hello" },
                    new TranslationDTO { LanguageCode = "en", Value = "Duplicate" },
                    new TranslationDTO { LanguageCode = "es", Value = "Hola" }
                },
                Author = new CharacterDTO(),
                Episode = new EpisodeDTO { Number = 1, Arc = new ArcDTO { Id = 1, FillerType = FillerType.CANON, Saga = new SagaDTO { Id = 1 } } }
            };

            mediatorMock.Setup(m => m.Send(It.IsAny<GetQuoteQuery>(), default)).ReturnsAsync(quote);
            mediatorMock.Setup(m => m.Send(It.IsAny<ListCharactersQuery>(), default)).ReturnsAsync(new List<CharacterDTO>());
            mediatorMock.Setup(m => m.Send(It.IsAny<ListEpisodesQuery>(), default)).ReturnsAsync(new List<EpisodeDTO>());
            mediatorMock.Setup(m => m.Send(It.IsAny<ListArcsQuery>(), default)).ReturnsAsync(new List<ArcDTO>());
            mediatorMock.Setup(m => m.Send(It.IsAny<ListSagasQuery>(), default)).ReturnsAsync(new List<SagaDTO>());

            // Act
            var result = await controller.Get(1) as PartialViewResult;
            var model = result?.Model as QuoteViewModel;

            // Assert
            model.Should().NotBeNull();
            model!.Translations.Should().ContainKey("en");
            model.Translations.Should().ContainKey("es");
            model.Translations.Should().NotContainKey(string.Empty);
            model.Translations["en"].Should().Be("Hello");
        }

        [Test]
        public async Task Post_WhenNewAuthorAndEpisode_ShouldCallSaveCharacterAndSaveEpisodeCommands()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var configurationMock = new Mock<IConfiguration>();

            var controller = new QuoteFormController(mediatorMock.Object, configurationMock.Object);
            var videoStream = new MemoryStream();
            var writer = new StreamWriter(videoStream);
            writer.Write("fake video content");
            writer.Flush();
            videoStream.Position = 0;

            var videoStreamEs = new MemoryStream();
            var writerEs = new StreamWriter(videoStreamEs);
            writerEs.Write("fake video content es");
            writerEs.Flush();
            videoStreamEs.Position = 0;

            var formFile = new FormFile(videoStream, 0, videoStream.Length, "VideoFile", "test.mp4")
            {
                Headers = new HeaderDictionary(),
                ContentType = "video/mp4"
            };
            var formFileEs = new FormFile(videoStreamEs, 0, videoStreamEs.Length, "VideoFileEs", "test-es.mp4")
            {
                Headers = new HeaderDictionary(),
                ContentType = "video/mp4"
            };


            var request = new QuoteFormPostRequestModel
            {
                AuthorId = -1,
                EpisodeNumber = -1,
                ArcId = -1,
                SagaId = -1,
                NewAuthorName = "Luffy",
                NewEpisodeNumber = 100,
                NewEpisodeTitles = new Dictionary<string, string> { { "en", "The Pirate King" } },
                NewArcTitles = new Dictionary<string, string> { { "en", "East Blue Saga" } },
                NewArcFillerTypeId = 1,
                NewSagaTitles = new Dictionary<string, string> { { "en", "One Piece" } },
                OriginalText = "Ore wa Kaizoku Ō ni naru!",
                Text = "I will become the Pirate King!",
                Translations = new Dictionary<string, string> { { "es", "¡Seré el Rey de los Piratas!" } },
                VideoFile = formFile,
                VideoFileEs = formFileEs
            };

            // Setup expected returns from mocked commands
            var saga = new SagaDTO { Id = 31 };
            var arc = new ArcDTO { Id = 227 };
            var character = new CharacterDTO { Id = 42 };
            var quote = new QuoteDTO { Id = 57 };

            mediatorMock
                .Setup(m => m.Send(It.IsAny<SaveSagaCommand>(), default))
                .Callback<IRequest, CancellationToken>((cmd, _) =>
                {
                    ((SaveSagaCommand)cmd).Saga.Id = saga.Id;
                });

            mediatorMock
                .Setup(m => m.Send(It.IsAny<SaveArcCommand>(), default))
                .Callback<IRequest, CancellationToken>((cmd, _) =>
                {
                    ((SaveArcCommand)cmd).Arc.Id = arc.Id;
                });

            mediatorMock
                .Setup(m => m.Send(It.IsAny<SaveCharacterCommand>(), default))
                .Callback<IRequest, CancellationToken>((cmd, _) =>
                {
                    ((SaveCharacterCommand)cmd).Character.Id = character.Id;
                });

            mediatorMock
                .Setup(m => m.Send(It.IsAny<SaveEpisodeCommand>(), default))
                .Callback<IRequest, CancellationToken>((cmd, _) =>
                {
                    ((SaveEpisodeCommand)cmd).Episode.Number = request.NewEpisodeNumber ?? 0;
                });

            mediatorMock
                .Setup(m => m.Send(It.IsAny<SaveQuoteCommand>(), default))
                .Callback<IRequest, CancellationToken>((cmd, _) =>
                {
                    ((SaveQuoteCommand)cmd).Quote.Id = quote.Id;
                });

            // Act
            var result = await controller.Post(request);

            // Assert
            mediatorMock.Verify(m => m.Send(It.IsAny<SaveCharacterCommand>(), default), Times.Once);
            mediatorMock.Verify(m => m.Send(It.IsAny<SaveSagaCommand>(), default), Times.Once);
            mediatorMock.Verify(m => m.Send(It.IsAny<SaveArcCommand>(), default), Times.Once);
            mediatorMock.Verify(m => m.Send(It.IsAny<SaveEpisodeCommand>(), default), Times.Once);
            mediatorMock.Verify(m => m.Send(It.IsAny<SaveQuoteCommand>(), default), Times.Once);

            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(quote.Id);
        }
    }
}
