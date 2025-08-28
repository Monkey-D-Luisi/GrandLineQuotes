using Application.Contexts.Quotes.Commands;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using Tests.Core;
using GrandLineQuotes.Client.Abstractions.DTOs.Common;

namespace Application.Tests.Unit.Contexts.Quotes.Commands
{
    [TestFixture(Category = "Unit")]
    internal class SaveQuoteCommandShould : TestBase
    {
        [Test]
        public void Create_quote_with_video_when_id_is_zero_and_video_is_valid()
        {
            using var stream = new MemoryStream(new byte[] { 1 });
            var videos = new Dictionary<string, VideoDTO>
            {
                { LanguagePath.English, new VideoDTO { Content = stream, ContentType = "video/mp4" } },
                { LanguagePath.Spanish, new VideoDTO { Content = new MemoryStream(new byte[] { 2 }), ContentType = "video/mp4" } }
            };
            var command = new SaveQuoteCommand(
                id: 0,
                originalText: "original",
                text: "text",
                translations: new Dictionary<string, string> { { "en", "text" } },
                authorId: 1,
                episodeNumber: 1,
                isReviewed: false,
                videos: videos);

            command.Quote.Videos?[LanguagePath.English].Content.Should().BeSameAs(stream);
        }
    }
}
