using Admin.Controllers.Arcs.Forms;
using Admin.Models.Arcs.Forms;
using Application.Contexts.Arcs.Commands;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Admin.Tests.Unit.Controllers.Arcs.Forms
{
    [TestFixture(Category = "Unit")]
    internal class ArcFormControllerShould
    {
        [Test]
        public async Task Post_ShouldSendSaveArcCommand()
        {
            var mediatorMock = new Mock<IMediator>();
            var controller = new ArcFormController(mediatorMock.Object);
            var request = new ArcFormPostModel
            {
                Id = 1,
                Titles = new Dictionary<string, string>() { { "en", "test title"} },
                FillerType = FillerType.CANON,
                SagaId = 2
            };

            var result = await controller.Post(request);

            mediatorMock.Verify(m => m.Send(It.IsAny<SaveArcCommand>(), default), Times.Once);
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().Be(request.Id);
        }
    }
}
