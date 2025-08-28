using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using System.Net;
using System.Net.Http.Json;
using Tests.Core;

namespace Api.Tests.Acceptance.Controllers.Quotes
{
    [TestFixture(Category = "Acceptance")]
    internal class QuotesControllerShould : AcceptanceTestBase<Program>
    {


        [Test]
        public async Task Return_an_array_of_quotes()
        {
            // Arrange
            var endpoint = "v1/quotes";

            // Act
            var response = await WebApplicationClient.GetAsync(endpoint);
            // Deserialize response body as list of quotes
            var quotes = await response.Content.ReadFromJsonAsync<IEnumerable<QuoteDTO>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            quotes.Should().NotBeNullOrEmpty();
        }


        [Test]
        public async Task Return_a_quote_given_a_id()
        {
            // Arrange
            var endpoint = $"v1/quotes/{QuoteId}";

            // Act
            var response = await WebApplicationClient.GetAsync(endpoint);
            // Deserialize response body as a quote
            var quote = await response.Content.ReadFromJsonAsync<QuoteDTO>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            quote.Should().NotBeNull();
            quote!.Id.Should().Be(QuoteId);
        }
    }
}
