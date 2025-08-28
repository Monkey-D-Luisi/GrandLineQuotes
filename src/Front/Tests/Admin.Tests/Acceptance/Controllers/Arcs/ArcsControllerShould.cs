using FluentAssertions;
using System.Net;
using Tests.Core;

namespace Admin.Tests.Acceptance.Controllers.Arcs
{
    [TestFixture(Category = "Acceptance")]
    internal class ArcsControllerShould : AcceptanceTestBase<Program>
    {
        [Test]
        public async Task Return_the_arcs_admin_main_view()
        {
            // Arrange
            var endpoint = "arcs";

            // Act
            var response = await WebApplicationClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}

