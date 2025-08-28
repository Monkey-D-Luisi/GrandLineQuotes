using Api;
using Client.Abstractions.Clients;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using GrandLineQuotes.Client.Abstractions.RequestModels.Quotes;
using Microsoft.Extensions.DependencyInjection;
using Tests.Core;

namespace Client.Tests.Acceptance.Clients
{
    [TestFixture(Category = "Acceptance")]
    internal class ApiClientShould : ClientAcceptanceTestBase<Program>
    {


        private IApiClient apiClient;


        [SetUp]
        public new void SetUp()
        {
            apiClient = WebApplicationFactory.Services.GetRequiredService<IApiClient>();
        }


        [Test]
        public async Task Return_an_array_of_quotes()
        {
            var requestModel = new QuotesListRequestModel
            {
                ArcId = 1,
                AuthorId = 1,
                SearchTerm = "hisá"
            };

            var response = await apiClient.ListQuotes(requestModel);

            response.Should().BeAssignableTo<IEnumerable<QuoteDTO>>();
        }


        [Test]
        public async Task Return_a_quote_detail_given_an_id()
        {
            var response = await apiClient.GetQuote(QuoteId);

            response.Should().BeAssignableTo<QuoteDTO>();
        }
    }
}
