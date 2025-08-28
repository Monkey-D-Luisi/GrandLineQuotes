using FluentAssertions;
using Flurl;
using System.Net;
using Tests.Core;

namespace Admin.Tests.Acceptance.Controllers.Quotes
{
    [TestFixture(Category = "Acceptance")]
    internal class QuotesControllerShould : AcceptanceTestBase<Program>
    {


        [Test]
        public async Task Return_the_quotes_admin_main_view()
        {
            // Arrange
            var endpoint = "quotes"
                .SetQueryParam("authorId", 1)
                .SetQueryParam("arcId", 1)
                .SetQueryParam("searchTerm", "hisáshiburi");

            // Act
            var response = await WebApplicationClient.GetAsync(endpoint);
            var contetn = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
