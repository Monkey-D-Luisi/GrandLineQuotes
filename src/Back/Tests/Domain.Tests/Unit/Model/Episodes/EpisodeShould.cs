using Domain.Model.Arcs;
using Domain.Model.Episodes;
using Domain.Model.Translations;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Arcs;
using GrandLineQuotes.Client.Abstractions.DTOs.Episodes;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using Moq;
using Tests.Core;
using System;

namespace Domain.Tests.Unit.Model.Episodes
{
    [TestFixture(Category = "Unit")]
    internal class EpisodeShould : TestBase
    {
        private const int testEpisodeNumber = 1;
        private const int testArcId = 2;

        private readonly List<Translation> testTitles;
        private readonly List<TranslationDTO> testTitleDTOs;
        private readonly Mock<Arc> testArc;
        private readonly ArcDTO testArcDTO;
        private readonly Mock<Episode> testEpisode;
        private readonly EpisodeDTO testEpisodeDTO;

        public EpisodeShould()
        {
            var translationDTO = new TranslationDTO { ParentId = null, LanguageCode = "en", Value = "Title" };
            var translation = Translation.CreateFrom(translationDTO);
            testTitles = new List<Translation> { translation };
            testTitleDTOs = new List<TranslationDTO> { translationDTO };

            testArcDTO = new ArcDTO { Id = testArcId };
            testArc = new Mock<Arc>();
            testArc.SetupGet(x => x.Id).Returns(testArcId);

            testEpisode = new Mock<Episode>();
            testEpisode.SetupGet(x => x.Number).Returns(testEpisodeNumber);
            testEpisode.SetupGet(x => x.Titles).Returns(testTitles);
            testEpisode.SetupGet(x => x.ArcId).Returns(testArcId);
            testEpisode.SetupGet(x => x.Arc).Returns(testArc.Object);

            testEpisodeDTO = new EpisodeDTO
            {
                Number = testEpisodeNumber,
                Titles = testTitleDTOs,
                ArcId = testArcId,
                Arc = testArcDTO
            };
        }

        [Test]
        public void Return_the_entity_snapshot()
        {
            var snapshot = testEpisode.Object.GetSnapshot();
            snapshot.Should().BeEquivalentTo(testEpisodeDTO);
        }

        [Test]
        public void Be_created_from_a_snapshot()
        {
            var episode = Episode.CreateFrom(testEpisodeDTO);
            episode.Should().BeEquivalentTo(testEpisode.Object);
        }

        [Test]
        public void Assign_arc()
        {
            var episode = new Episode();
            var arc = Arc.CreateFrom(testArcDTO);
            episode.AssignArc(arc);
            episode.Arc.Should().Be(arc);
            episode.GetSnapshot().ArcId.Should().Be(arc.Id);
        }

        [Test]
        public void Throw_when_assigning_null_arc()
        {
            var episode = new Episode();
            Action act = () => episode.AssignArc(null!);
            act.Should().Throw<ArgumentNullException>();
        }

    }
}
