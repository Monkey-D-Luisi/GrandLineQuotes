using FluentAssertions;
using System.Net;
using Tests.Core;

namespace Admin.Tests.Acceptance.Controllers.Sagas
{
    [TestFixture(Category = "Acceptance")]
    internal class SagasControllerShould : AcceptanceTestBase<Program>
    {
        [Test]
        public async Task Return_the_sagas_admin_main_view()
        {
            // Arrange
            var endpoint = "sagas";

            // Act
            var response = await WebApplicationClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}

