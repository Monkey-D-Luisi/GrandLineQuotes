using Domain.Model.Characters;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Characters;
using Infrastructure.Contexts.Characters.Repositories;
using Tests.Core;

namespace Infrastructure.Tests.Integration.Contexts.Characters.Repositories
{
    [TestFixture(Category = "Integration")]
    internal class CharacterRepositoryShould : IntegrationTestBase
    {


        [Test]
        public async Task Should_Create_Get_Update_And_Delete_Character()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            var repository = new CharacterRepository(dbContext);

            var dto = new CharacterDTO
            {
                Name = "Zoro",
                Alias = "Pirate Hunter"
            };

            var character = Character.CreateFrom(dto);

            // Act - Save (Create)
            await repository.Save(character, cancellationToken);
            var createdId = character.Id;

            // Assert - Get after create
            var createdCharacter = await repository.Get(createdId, cancellationToken, tracked: true);
            createdCharacter.Should().NotBeNull();
            createdCharacter!.Name.Should().Be("Zoro");
            createdCharacter.Alias.Should().Be("Pirate Hunter");

            // Act - Update
            dto = createdCharacter.GetSnapshot();
            dto.Name = "Roronoa Zoro";
            dto.Alias = "Mr. Bushido";
            createdCharacter.UpdateFrom(dto);
            await repository.Save(createdCharacter, cancellationToken);

            // Assert - Get after update
            var updatedCharacter = await repository.Get(createdId, cancellationToken);
            updatedCharacter.Should().NotBeNull();
            updatedCharacter!.Name.Should().Be("Roronoa Zoro");
            updatedCharacter.Alias.Should().Be("Mr. Bushido");

            // Act - Delete
            await repository.Delete(createdId, cancellationToken);

            // Assert - Get after delete
            var deletedCharacter = await repository.Get(createdId, cancellationToken);
            deletedCharacter.Should().BeNull();
        }
    }
}
