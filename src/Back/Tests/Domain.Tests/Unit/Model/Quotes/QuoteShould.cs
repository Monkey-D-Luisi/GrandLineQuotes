using Domain.Model.Characters;
using Domain.Model.Episodes;
using Domain.Model.Quotes;
using Domain.Model.Translations;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Characters;
using GrandLineQuotes.Client.Abstractions.DTOs.Episodes;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using Moq;
using Tests.Core;
using System;

namespace Domain.Tests.Unit.Model.Quotes
{
    [TestFixture(Category = "Unit")]
    internal class QuoteShould : TestBase
    {
        private const int testQuoteId = 1;
        private const string testOriginalText = "Original";
        private const string testQuoteText = "Translated";
        private const int testAuthorId = 2;
        private const string testAuthorName = "Author";
        private const string testAuthorAlias = "Alias";
        private const int testEpisodeNumber = 3;
        private const bool testIsReviewed = true;

        private readonly List<Translation> testTranslations;
        private readonly List<TranslationDTO> testTranslationDTOs;
        private readonly Mock<Character> testAuthor;
        private readonly CharacterDTO testAuthorDTO;
        private readonly Mock<Episode> testEpisode;
        private readonly EpisodeDTO testEpisodeDTO;
        private readonly Mock<Quote> testQuote;
        private readonly QuoteDTO testQuoteDTO;

        public QuoteShould()
        {
            var translationDTO = new TranslationDTO
            {
                ParentId = null,
                LanguageCode = "en",
                Value = testQuoteText
            };
            var translation = Translation.CreateFrom(translationDTO);
            testTranslations = new List<Translation> { translation };
            testTranslationDTOs = new List<TranslationDTO> { translationDTO };

            testAuthor = new Mock<Character>();
            testAuthor.SetupGet(x => x.Id).Returns(testAuthorId);
            testAuthor.SetupGet(x => x.Name).Returns(testAuthorName);
            testAuthor.SetupGet(x => x.Alias).Returns(testAuthorAlias);
            testAuthorDTO = new CharacterDTO { Id = testAuthorId, Name = testAuthorName, Alias = testAuthorAlias };

            testEpisode = new Mock<Episode>();
            testEpisode.SetupGet(x => x.Number).Returns(testEpisodeNumber);
            testEpisodeDTO = new EpisodeDTO { Number = testEpisodeNumber };

            testQuote = new Mock<Quote>();
            testQuote.SetupGet(x => x.Id).Returns(testQuoteId);
            testQuote.SetupGet(x => x.OriginalText).Returns(testOriginalText);
            testQuote.SetupGet(x => x.Text).Returns(testQuoteText);
            testQuote.SetupGet(x => x.Translations).Returns(testTranslations);
            testQuote.SetupGet(x => x.AuthorId).Returns(testAuthorId);
            testQuote.SetupGet(x => x.Author).Returns(testAuthor.Object);
            testQuote.SetupGet(x => x.EpisodeNumber).Returns(testEpisodeNumber);
            testQuote.SetupGet(x => x.Episode).Returns(testEpisode.Object);
            testQuote.SetupGet(x => x.IsReviewed).Returns(testIsReviewed);

            testQuoteDTO = new QuoteDTO
            {
                Id = testQuoteId,
                OriginalText = testOriginalText,
                Text = testQuoteText,
                Translations = testTranslationDTOs,
                AuthorId = testAuthorId,
                Author = testAuthorDTO,
                EpisodeNumber = testEpisodeNumber,
                Episode = testEpisodeDTO,
                IsReviewed = testIsReviewed
            };
        }

        [Test]
        public void Return_the_entity_snapshot()
        {
            var snapshot = testQuote.Object.GetSnapshot();
            snapshot.Should().BeEquivalentTo(testQuoteDTO);
        }

        [Test]
        public void Be_created_from_a_snapshot()
        {
            var quote = Quote.CreateFrom(testQuoteDTO);
            quote.Should().BeEquivalentTo(testQuote.Object);
        }

        [Test]
        public void Assign_author_to_quote()
        {
            var quote = new Quote();
            var author = Character.CreateFrom(testAuthorDTO);
            quote.AssignAuthor(author);
            quote.Author.Should().Be(author);
            quote.GetSnapshot().AuthorId.Should().Be(author.Id);
        }

        [Test]
        public void Throw_when_assigning_null_author()
        {
            var quote = new Quote();
            Action act = () => quote.AssignAuthor(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Review_quote()
        {
            var quote = new Quote();
            quote.SetOriginalText(testOriginalText);
            quote.Review();
            quote.IsReviewed.Should().BeTrue();
        }

        [Test]
        public void Throw_when_reviewing_without_text()
        {
            var quote = new Quote();
            Action act = () => quote.Review();
            act.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void Throw_when_reviewing_twice()
        {
            var quote = new Quote();
            quote.SetOriginalText(testOriginalText);
            quote.Review();
            Action act = () => quote.Review();
            act.Should().Throw<InvalidOperationException>();
        }
    }
}

