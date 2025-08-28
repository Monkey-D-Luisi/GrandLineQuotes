using Application.Contexts.Characters.Services;
using Domain.Model.Characters;
using Domain.Model.Characters.Abstractions;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Characters;
using Infrastructure.Contexts.Characters.Services;
using Moq;

namespace Infrastructure.Tests.Unit.Contexts.Characters.Services
{
    [TestFixture(Category = "Unit")]
    public class CharacterServiceShould
    {


        private readonly Mock<ICharacterRepository> repositoryMock;
        private readonly CharacterService service;


        public CharacterServiceShould()
        {
            repositoryMock = new Mock<ICharacterRepository>();
            service = new CharacterService(repositoryMock.Object);
        }


        [Test]
        public async Task Save_Should_Create_New_Character_When_Id_Is_Zero()
        {
            // Arrange
            var dto = new CharacterDTO { Id = 0, Name = "Luffy" };
            repositoryMock.Setup(r => r.Get(It.IsAny<int>(), It.IsAny<CancellationToken>(), true))
                .ReturnsAsync((Character?)null);

            var savedDto = new CharacterDTO { Id = 42, Name = "Luffy" };

            repositoryMock.Setup(r => r.Save(It.IsAny<Character>(), It.IsAny<CancellationToken>()))
                .Callback<Character, CancellationToken>((character, _) =>
                {
                    typeof(Character).GetProperty("Id")!
                        .SetValue(character, savedDto.Id);
                })
                .Returns(Task.CompletedTask);

            // Act
            await service.Save(dto, CancellationToken.None);

            // Assert
            repositoryMock.Verify(r => r.Get(0, It.IsAny<CancellationToken>(), true), Times.Never);
            repositoryMock.Verify(r => r.Save(It.IsAny<Character>(), It.IsAny<CancellationToken>()), Times.Once);
            dto.Id.Should().Be(savedDto.Id);
        }


        [Test]
        public async Task Save_Should_Update_Existing_Character_When_Id_Is_Not_Zero()
        {
            // Arrange
            var dto = new CharacterDTO { Id = 10, Name = "Zoro" };

            var existingCharacter = Character.CreateFrom(dto);

            repositoryMock.Setup(r => r.Get(10, It.IsAny<CancellationToken>(), true))
                          .ReturnsAsync(existingCharacter);

            repositoryMock.Setup(r => r.Save(existingCharacter, It.IsAny<CancellationToken>()))
                          .Returns(Task.CompletedTask);

            // Act
            await service.Save(dto, CancellationToken.None);

            // Assert
            repositoryMock.Verify(r => r.Get(10, It.IsAny<CancellationToken>(), true), Times.Once);
            repositoryMock.Verify(r => r.Save(existingCharacter, It.IsAny<CancellationToken>()), Times.Once);
            dto.Id.Should().Be(dto.Id);
        }
    }
}
