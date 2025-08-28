using Admin.Controllers.Characters.Forms;
using Admin.Models.Characters.Forms;
using Application.Contexts.Characters.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Admin.Tests.Unit.Controllers.Characters.Forms
{
    [TestFixture(Category = "Unit")]
    internal class CharacterFormControllerShould
    {
        [Test]
        public async Task Post_ShouldSendSaveCharacterCommand()
        {
            var mediatorMock = new Mock<IMediator>();
            var controller = new CharacterFormController(mediatorMock.Object);
            var request = new CharacterFormPostModel
            {
                Id = 1,
                Name = "Luffy"
            };

            var result = await controller.Post(request);

            mediatorMock.Verify(m => m.Send(It.IsAny<SaveCharacterCommand>(), default), Times.Once);
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().Be(request.Id);
        }
    }
}
