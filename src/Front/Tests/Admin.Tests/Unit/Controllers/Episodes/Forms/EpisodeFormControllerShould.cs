using Admin.Controllers.Episodes.Forms;
using Admin.Models.Episodes.Forms;
using Application.Contexts.Episodes.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Admin.Tests.Unit.Controllers.Episodes.Forms
{
    [TestFixture(Category = "Unit")]
    internal class EpisodeFormControllerShould
    {
        [Test]
        public async Task Post_ShouldSendSaveEpisodeCommand()
        {
            var mediatorMock = new Mock<IMediator>();
            var controller = new EpisodeFormController(mediatorMock.Object);
            var request = new EpisodeFormPostModel
            {
                Number = 1,
                Titles = new Dictionary<string, string>() { { "en", "test title" } },
                ArcId = 1
            };

            var result = await controller.Post(request);

            mediatorMock.Verify(m => m.Send(It.IsAny<SaveEpisodeCommand>(), default), Times.Once);
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().Be(request.Number);
        }
    }
}
