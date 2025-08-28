using FluentAssertions;
using Flurl;
using System.Net;
using System.Net.Http.Headers;
using Tests.Core;

namespace Admin.Tests.Acceptance.Controllers.Quotes.Forms
{
    internal class QuoteFormControllerShould : AcceptanceTestBase<Program>
    {


        [Test]
        public async Task Return_the_empty_quote_form()
        {
            // Arrange
            var endpoint = "quotes/form";

            // Act
            var response = await WebApplicationClient.GetAsync(endpoint);
            var contetn = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        [Test]
        public async Task Return_the_informed_quote_form()
        {
            // Arrange
            var endpoint = $"quotes/form/{QuoteId}";

            // Act
            var response = await WebApplicationClient.GetAsync(endpoint);
            var contetn = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        [Test]
        public async Task Create_a_new_quoteand_delete_it()
        {
            // Arrange
            var insertPath = $"quotes/form";
            var deletePath = $"quotes";
            var videoPath = Path.Combine("Files", "1.mp4");

            using var videoStream = File.OpenRead(videoPath);
            StreamContent videoContent = new(videoStream);
            videoContent.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
            using var videoStreamEs = File.OpenRead(videoPath);
            StreamContent videoContentEs = new(videoStreamEs);
            videoContentEs.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");

            var payload = new MultipartFormDataContent
            {
                { new StringContent("Test quote"), "OriginalText" },
                { new StringContent("Test quote"), "Text" },
                { new StringContent("Test quote"), "Translations[en]" },
                { new StringContent("Test cita"), "Translations[es]" },
                { new StringContent("1"), "AuthorId" },
                { new StringContent("442"), "EpisodeNumber" },
                { videoContent, "VideoFile", "test.mp4" },
                { videoContentEs, "VideoFileEs", "test-es.mp4" }
            };

            // Act
            var insertResponse = await WebApplicationClient.PostAsync(insertPath, payload);
            var quoteId = await insertResponse.Content.ReadAsStringAsync();

            deletePath = deletePath
                .AppendPathSegment(quoteId);

            var deleteResponse = await WebApplicationClient.DeleteAsync(deletePath);

            // Assert
            insertResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        [Test]
        public async Task Throw_bad_request_when_missing_data()
        {
            // Arrange
            var insertPath = $"quotes/form";

            var payload = new MultipartFormDataContent
            {
                { new StringContent("Test quote"), "OriginalText" }
            };

            // Act
            var insertResponse = await WebApplicationClient.PostAsync(insertPath, payload);

            // Assert
            insertResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
