using FluentAssertions;
using Flurl;
using System.Net;
using Tests.Core;

namespace Admin.Tests.Acceptance.Controllers.Episodes.Forms
{
    internal class EpisodeFormControllerShould : AcceptanceTestBase<Program>
    {
        [Test]
        public async Task Return_the_empty_episode_form()
        {
            var endpoint = "episodes/form";

            var response = await WebApplicationClient.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Return_the_informed_episode_form()
        {
            var endpoint = "episodes/form/1";

            var response = await WebApplicationClient.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Create_a_new_episode_and_delete_it()
        {
            var insertPath = "episodes/form";
            var deletePath = "episodes";

            var payload = new MultipartFormDataContent
            {
                { new StringContent("9999"), "Number" },
                { new StringContent("Test episode"), "Titles[en]" },
                { new StringContent("1"), "ArcId" }
            };

            var insertResponse = await WebApplicationClient.PostAsync(insertPath, payload);
            var episodeNumber = await insertResponse.Content.ReadAsStringAsync();

            deletePath = deletePath.AppendPathSegment(episodeNumber);

            var deleteResponse = await WebApplicationClient.DeleteAsync(deletePath);

            insertResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Throw_bad_request_when_missing_data()
        {
            var insertPath = "episodes/form";

            var payload = new MultipartFormDataContent
            {
                { new StringContent("1234"), "Number" },
                { new StringContent("1"), "ArcId" }
            };

            var insertResponse = await WebApplicationClient.PostAsync(insertPath, payload);

            insertResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}

