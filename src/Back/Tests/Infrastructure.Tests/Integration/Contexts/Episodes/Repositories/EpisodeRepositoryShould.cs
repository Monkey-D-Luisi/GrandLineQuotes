using Domain.Model.Episodes;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Episodes;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using Infrastructure.Contexts.Episodes.Repositories;
using Tests.Core;

namespace Infrastructure.Tests.Integration.Contexts.Episodes.Repositories
{
    [TestFixture(Category = "Integration")]
    internal class EpisodeRepositoryShould : IntegrationTestBase
    {


        [Test]
        public async Task Should_Create_Get_Update_And_Delete_Episode()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            var repository = new EpisodeRepository(dbContext);

            var dto = new EpisodeDTO
            {
                Number = 999999,
                Titles = new List<TranslationDTO>
                {
                    new TranslationDTO { LanguageCode = "en", Value = "The Pirate King" }
                },
            };

            var episode = Episode.CreateFrom(dto);

            // Act - Create
            await repository.Create(episode, cancellationToken);

            // Assert - Get after create
            var created = await repository.Get(dto.Number, cancellationToken, tracked: true);
            created.Should().NotBeNull();
            created!.Titles.First(title => title.LanguageCode == "en").Value.Should().Be("The Pirate King");

            // Act - Update
            dto = created.GetSnapshot();
            var createdTitle = dto.Titles.First(title => title.LanguageCode == "en");
            createdTitle.Value = "I Will Be King";
            created.UpdateFrom(dto);
            await repository.Update(created, cancellationToken);

            // Assert - Get after update
            var updated = await repository.Get(dto.Number, cancellationToken);
            updated.Should().NotBeNull();
            updated!.Titles.First(title => title.LanguageCode == "en").Value.Should().Be("I Will Be King");

            // Act - Delete
            await repository.Delete(dto.Number, cancellationToken);

            // Assert - Get after delete
            var deleted = await repository.Get(dto.Number, cancellationToken);
            deleted.Should().BeNull();
        }

    }
}
