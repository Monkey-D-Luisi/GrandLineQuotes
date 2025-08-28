using FluentAssertions;
using System.Net;
using Tests.Core;

namespace Admin.Tests.Acceptance.Controllers.Characters
{
    [TestFixture(Category = "Acceptance")]
    internal class CharactersControllerShould : AcceptanceTestBase<Program>
    {
        [Test]
        public async Task Return_the_characters_admin_main_view()
        {
            // Arrange
            var endpoint = "characters";

            // Act
            var response = await WebApplicationClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}

