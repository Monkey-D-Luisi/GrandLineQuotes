using FluentAssertions;
using Flurl;
using System.Net;
using Tests.Core;

namespace Admin.Tests.Acceptance.Controllers.Characters.Forms
{
    internal class CharacterFormControllerShould : AcceptanceTestBase<Program>
    {
        [Test]
        public async Task Return_the_empty_character_form()
        {
            var endpoint = "characters/form";

            var response = await WebApplicationClient.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Return_the_informed_character_form()
        {
            var endpoint = "characters/form/1";

            var response = await WebApplicationClient.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Create_a_new_character_and_delete_it()
        {
            var insertPath = "characters/form";
            var deletePath = "characters";

            var payload = new MultipartFormDataContent
            {
                { new StringContent("Test character"), "Name" }
            };

            var insertResponse = await WebApplicationClient.PostAsync(insertPath, payload);
            var characterId = await insertResponse.Content.ReadAsStringAsync();

            deletePath = deletePath.AppendPathSegment(characterId);

            var deleteResponse = await WebApplicationClient.DeleteAsync(deletePath);

            insertResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Throw_bad_request_when_missing_data()
        {
            var insertPath = "characters/form";

            var payload = new MultipartFormDataContent();

            var insertResponse = await WebApplicationClient.PostAsync(insertPath, payload);

            insertResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}

