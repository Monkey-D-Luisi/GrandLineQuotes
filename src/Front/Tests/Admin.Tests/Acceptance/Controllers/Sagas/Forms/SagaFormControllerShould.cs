using FluentAssertions;
using Flurl;
using System.Net;
using Tests.Core;

namespace Admin.Tests.Acceptance.Controllers.Sagas.Forms
{
    internal class SagaFormControllerShould : AcceptanceTestBase<Program>
    {
        [Test]
        public async Task Return_the_empty_saga_form()
        {
            var endpoint = "sagas/form";

            var response = await WebApplicationClient.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Return_the_informed_saga_form()
        {
            var endpoint = "sagas/form/1";

            var response = await WebApplicationClient.GetAsync(endpoint);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Create_a_new_saga_and_delete_it()
        {
            var insertPath = "sagas/form";
            var deletePath = "sagas";

            var payload = new MultipartFormDataContent
            {
                { new StringContent("Test saga"), "Titles[en]" }
            };

            var insertResponse = await WebApplicationClient.PostAsync(insertPath, payload);
            var sagaId = await insertResponse.Content.ReadAsStringAsync();

            deletePath = deletePath.AppendPathSegment(sagaId);

            var deleteResponse = await WebApplicationClient.DeleteAsync(deletePath);

            insertResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Throw_bad_request_when_missing_data()
        {
            var insertPath = "sagas/form";

            var payload = new MultipartFormDataContent();

            var insertResponse = await WebApplicationClient.PostAsync(insertPath, payload);

            insertResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}

