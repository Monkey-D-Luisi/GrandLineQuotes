using Admin.Controllers.Sagas.Forms;
using Admin.Models.Sagas.Forms;
using Application.Contexts.Sagas.Commands;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Admin.Tests.Unit.Controllers.Sagas.Forms
{
    [TestFixture(Category = "Unit")]
    internal class SagaFormControllerShould
    {
        [Test]
        public async Task Post_ShouldSendSaveSagaCommand()
        {
            var mediatorMock = new Mock<IMediator>();
            var controller = new SagaFormController(mediatorMock.Object);
            var request = new SagaFormPostModel
            {
                Id = 1,
                Titles = new Dictionary<string, string>() { { "en", "test title" } }
            };

            var result = await controller.Post(request);

            mediatorMock.Verify(m => m.Send(It.IsAny<SaveSagaCommand>(), default), Times.Once);
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().Be(request.Id);
        }
    }
}
