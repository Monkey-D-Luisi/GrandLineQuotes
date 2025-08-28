using FluentAssertions;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Tests.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admin.Tests.Acceptance.Controllers.Quotes
{
    [TestFixture(Category = "Acceptance")]
    internal class GraphQLShould : AcceptanceTestBase<Program>
    {
        protected override void ConfigureServices(IWebHostBuilder builder)
        {
            base.ConfigureServices(builder);
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<Client.Abstractions.Clients.IApiClient, FakeApiClient>();
                services.AddSingleton<Api.Public.Security.IPlayIntegrityService, FakePlayIntegrityService>();
            });
        }

        private class FakeApiClient : Client.Abstractions.Clients.IApiClient
        {
            public Task<IEnumerable<GrandLineQuotes.Client.Abstractions.DTOs.Quotes.QuoteDTO>> ListQuotes(GrandLineQuotes.Client.Abstractions.RequestModels.Quotes.QuotesListRequestModel requestModel)
            {
                var quote = new GrandLineQuotes.Client.Abstractions.DTOs.Quotes.QuoteDTO
                {
                    Id = 1,
                    Text = "Test quote",
                    Author = new GrandLineQuotes.Client.Abstractions.DTOs.Characters.CharacterDTO
                    {
                        Id = 1,
                        Name = "Luffy"
                    }
                };
                return Task.FromResult<IEnumerable<GrandLineQuotes.Client.Abstractions.DTOs.Quotes.QuoteDTO>>(new[] { quote });
            }

            public Task<GrandLineQuotes.Client.Abstractions.DTOs.Quotes.QuoteDTO> GetQuote(int quoteId)
            {
                var quote = new GrandLineQuotes.Client.Abstractions.DTOs.Quotes.QuoteDTO
                {
                    Id = quoteId,
                    Text = "Test quote",
                    Author = new GrandLineQuotes.Client.Abstractions.DTOs.Characters.CharacterDTO
                    {
                        Id = 1,
                        Name = "Luffy"
                    }
                };
                return Task.FromResult(quote);
            }
        }

        private class FakePlayIntegrityService : Api.Public.Security.IPlayIntegrityService
        {
            public Task<bool> ValidateAsync(string token, string nonce, string packageName) => Task.FromResult(token == nonce);
        }

        private async Task EnsureAuthorizedAsync()
        {
            var nonce = Guid.NewGuid().ToString();
            var request = new HttpRequestMessage(HttpMethod.Post, "integrity/exchange");
            request.Headers.Add("X-Play-Integrity-Token", nonce);
            request.Headers.Add("X-Play-Integrity-Nonce", nonce);
            request.Headers.Add("X-Play-Integrity-Package-Name", "com.grandlinequotes.app");

            var response = await WebApplicationClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var session = json.GetProperty("session").GetString();
            WebApplicationClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);
        }


        [Test]
        public async Task Expose_the_api()
        {
            // Arrange
            var endpoint = "health";

            // Act
            var response = await WebApplicationClient.GetAsync(endpoint);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        [Test]
        public async Task Expose_schema_introspection()
        {
            var introspectionQuery = new
            {
                query = "{ __schema { queryType { name } } }"
            };

            await EnsureAuthorizedAsync();
            var response = await WebApplicationClient.PostAsJsonAsync("graphql", introspectionQuery);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            json.GetProperty("data").GetProperty("__schema").GetProperty("queryType").GetProperty("name").GetString()
                .Should().Be("QuoteQueries");
        }


        [Test]
        public async Task ListQuotes_should_return_quotes()
        {
            var query = new
            {
                query = @"
                    query ($authorId: Int, $arcId: Int, $searchTerm: String) {
                        quotes(authorId: $authorId, arcId: $arcId, searchTerm: $searchTerm) {
                            id
                            text
                            author { 
                                id 
                                name 
                            }
                        }
                    }",
                variables = new
                {
                    authorId = 1,
                    arcId = 1,
                    searchTerm = "hisá"
                }
            };

            await EnsureAuthorizedAsync();
            var response = await WebApplicationClient.PostAsJsonAsync("graphql", query);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var quotes = json.GetProperty("data").GetProperty("quotes");
            quotes.GetArrayLength().Should().BeGreaterThan(0);
        }


        [Test]
        public async Task GetQuote_should_return_a_quote_given_an_id()
        {
            var query = new
            {
                query = @"
                    query {
                        quote(id: 1) {
                            id
                            text
                            author {
                                id
                                name
                            }
                        }
                    }"
            };

            await EnsureAuthorizedAsync();
            var response = await WebApplicationClient.PostAsJsonAsync("graphql", query);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var quote = json.GetProperty("data").GetProperty("quote");
            quote.GetProperty("id").GetInt32().Should().Be(1);
        }
    }
}
