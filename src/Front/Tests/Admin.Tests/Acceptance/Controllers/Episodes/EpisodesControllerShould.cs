using FluentAssertions;
using System.Net;
using Tests.Core;

namespace Admin.Tests.Acceptance.Controllers.Episodes
{
    [TestFixture(Category = "Acceptance")]
    internal class EpisodesControllerShould : AcceptanceTestBase<Program>
    {
        [Test]
        public async Task Return_the_episodes_admin_main_view()
        {
            // Arrange
            var endpoint = "episodes";

            // Act
            var response = await WebApplicationClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}

