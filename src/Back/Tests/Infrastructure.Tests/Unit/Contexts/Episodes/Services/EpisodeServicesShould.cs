using Domain.Model.Episodes;
using Domain.Model.Episodes.Repositories;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Episodes;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using Infrastructure.Contexts.Episodes.Services;
using Moq;

namespace Infrastructure.Tests.Unit.Contexts.Episodes.Services
{
    [TestFixture(Category = "Unit")]
    internal class EpisodeServicesShould
    {


        private readonly Mock<IEpisodeRepository> repositoryMock;
        private readonly EpisodeService service;


        public EpisodeServicesShould()
        {
            repositoryMock = new Mock<IEpisodeRepository>();
            service = new EpisodeService(repositoryMock.Object);
        }


        [Test]
        public async Task Save_ShouldCreateEpisode_WhenItDoesNotExist()
        {
            // Arrange
            var dto = new EpisodeDTO
            {
                Number = 101,
                Titles = new List<TranslationDTO>
                {
                    new TranslationDTO
                    {
                        LanguageCode = "en",
                        Value = "New World"
                    }
                }
            };

            repositoryMock.Setup(r => r.Get(101, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync((Episode?)null);

            Episode? createdEpisode = null;
            repositoryMock.Setup(r => r.Create(It.IsAny<Episode>(), It.IsAny<CancellationToken>()))
                .Callback<Episode, CancellationToken>((e, _) => createdEpisode = e)
                .Returns(Task.CompletedTask);

            // Act
            await service.Save(dto, CancellationToken.None);

            // Assert
            repositoryMock.Verify(r => r.Create(It.IsAny<Episode>(), It.IsAny<CancellationToken>()), Times.Once);
            createdEpisode.Should().NotBeNull();
            createdEpisode!.Number.Should().Be(101);
            createdEpisode.Titles.First(title => title.LanguageCode == "en").Value.Should().Be("New World");
        }


        [Test]
        public async Task Save_ShouldUpdateEpisode_WhenItExists()
        {
            // Arrange
            var dto = new EpisodeDTO
            {
                Number = 50,
                Titles = new List<TranslationDTO>
                {
                    new TranslationDTO
                    {
                        LanguageCode = "en",
                        Value = "Red Line"
                    }
                }
            };

            var existingEpisode = Episode.CreateFrom(dto);

            repositoryMock.Setup(r => r.Get(50, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(existingEpisode);

            repositoryMock.Setup(r => r.Update(existingEpisode, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await service.Save(dto, CancellationToken.None);

            // Assert
            repositoryMock.Verify();
            existingEpisode.Titles.First(title => title.LanguageCode == "en").Value.Should().Be("Red Line");
        }
    }
}
