using System;
using System.Collections.Generic;
using System.IO;
using Application.Contexts.Quotes.Commands;
using Application.Contexts.Quotes.Commands.Handlers;
using Application.Contexts.Quotes.Services;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tests.Core;
using GrandLineQuotes.Client.Abstractions.DTOs.Common;

namespace Application.Tests.Unit.Contexts.Quotes.Commands;

[TestFixture(Category = "Unit")]
internal class SaveQuoteCommandHandlerShould : TestBase
{
    private readonly Mock<IQuoteService> quoteServiceMock;
    private readonly Mock<IVideoService> videoServiceMock;
    private readonly SaveQuoteCommandHandler handler;

    public SaveQuoteCommandHandlerShould()
    {
        quoteServiceMock = new Mock<IQuoteService>();
        videoServiceMock = new Mock<IVideoService>();
        handler = new SaveQuoteCommandHandler(quoteServiceMock.Object, videoServiceMock.Object, NullLogger<SaveQuoteCommandHandler>.Instance);
    }

    [SetUp]
    public void Setup()
    {
        quoteServiceMock.Reset();
        videoServiceMock.Reset();
    }

    [Test]
    public async Task Throw_when_marked_reviewed_without_video()
    {
        // Arrange
        videoServiceMock
            .Setup(v => v.Exists(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = BuildCommand(id: QuoteId, isReviewed: true, videoStream: Stream.Null);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        videoServiceMock.Verify(v => v.Exists(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        quoteServiceMock.Verify(q => q.Save(It.IsAny<GrandLineQuotes.Client.Abstractions.DTOs.Quotes.QuoteDTO>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Delete_existing_video_and_upload_new_one_when_updating_quote()
    {
        // Arrange
        using var stream = new MemoryStream(new byte[] { 1 });
        videoServiceMock
            .Setup(v => v.Exists(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = BuildCommand(id: QuoteId, isReviewed: true, videoStream: stream);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        videoServiceMock.Verify(v => v.Delete($"{LanguagePath.English}/{QuoteId}.mp4", It.IsAny<CancellationToken>()), Times.Once);
        videoServiceMock.Verify(v => v.Delete($"{LanguagePath.Spanish}/{QuoteId}.mp4", It.IsAny<CancellationToken>()), Times.Once);
        videoServiceMock.Verify(v => v.Upload(It.IsAny<Stream>(), $"{LanguagePath.English}/{QuoteId}.mp4", "video/mp4", It.IsAny<CancellationToken>()), Times.Once);
        videoServiceMock.Verify(v => v.Upload(It.IsAny<Stream>(), $"{LanguagePath.Spanish}/{QuoteId}.mp4", "video/mp4", It.IsAny<CancellationToken>()), Times.Once);
        quoteServiceMock.Verify(q => q.Save(command.Quote, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Upload_video_when_creating_reviewed_quote_with_video()
    {
        // Arrange
        using var stream = new MemoryStream(new byte[] { 1 });
        var command = BuildCommand(id: 0, isReviewed: true, videoStream: stream);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        videoServiceMock.Verify(v => v.Upload(It.IsAny<Stream>(), It.IsAny<string>(), "video/mp4", It.IsAny<CancellationToken>()), Times.Exactly(2));
        quoteServiceMock.Verify(q => q.Save(command.Quote, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Save_without_video_operations_when_no_new_video_provided()
    {
        // Arrange
        videoServiceMock
            .Setup(v => v.Exists(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = BuildCommand(id: QuoteId, isReviewed: false, videoStream: Stream.Null);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        videoServiceMock.Verify(v => v.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        videoServiceMock.Verify(v => v.Upload(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        quoteServiceMock.Verify(q => q.Save(command.Quote, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static SaveQuoteCommand BuildCommand(int id, bool isReviewed, Stream videoStream) =>
        new SaveQuoteCommand(
            id: id,
            originalText: "original",
            text: "text",
            translations: new Dictionary<string, string> { { "en", "text" } },
            authorId: 1,
            episodeNumber: 1,
            isReviewed: isReviewed,
            videos: new Dictionary<string, VideoDTO>
            {
                { LanguagePath.English, new VideoDTO { Content = videoStream, ContentType = "video/mp4" } },
                { LanguagePath.Spanish, new VideoDTO { Content = videoStream, ContentType = "video/mp4" } }
            });
}

