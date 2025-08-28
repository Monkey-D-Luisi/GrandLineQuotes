using FluentAssertions;
using Flurl;
using System.Net;
using Tests.Core;

namespace Admin.Tests.Acceptance.Controllers.Arcs.Forms
{
    internal class ArcFormControllerShould : AcceptanceTestBase<Program>
    {
        [Test]
        public async Task Return_the_empty_arc_form()
        {
            var endpoint = "arcs/form";

            var response = await WebApplicationClient.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Return_the_informed_arc_form()
        {
            var endpoint = "arcs/form/1";

            var response = await WebApplicationClient.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Create_a_new_arc_and_delete_it()
        {
            var insertPath = "arcs/form";
            var deletePath = "arcs";

            var payload = new MultipartFormDataContent
            {
                { new StringContent("Test arc"), "Titles[en]" },
                { new StringContent("1"), "FillerType" },
                { new StringContent("1"), "SagaId" }
            };

            var insertResponse = await WebApplicationClient.PostAsync(insertPath, payload);
            var arcId = await insertResponse.Content.ReadAsStringAsync();

            deletePath = deletePath.AppendPathSegment(arcId);

            var deleteResponse = await WebApplicationClient.DeleteAsync(deletePath);

            insertResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Throw_bad_request_when_missing_data()
        {
            var insertPath = "arcs/form";

            var payload = new MultipartFormDataContent
            {
                { new StringContent("1"), "SagaId" }
            };

            var insertResponse = await WebApplicationClient.PostAsync(insertPath, payload);

            insertResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}

