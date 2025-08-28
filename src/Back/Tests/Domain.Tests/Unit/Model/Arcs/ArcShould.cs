using Domain.Model.Arcs;
using Domain.Model.Sagas;
using Domain.Model.Translations;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Arcs;
using GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using GrandLineQuotes.Client.Abstractions.DTOs.Sagas;
using Moq;
using Tests.Core;
using System;

namespace Domain.Tests.Unit.Model.Arcs
{
    [TestFixture(Category = "Unit")]
    internal class ArcShould : TestBase
    {
        private const int testArcId = 1;
        private const FillerType testFillerType = FillerType.CANON;
        private const int testSagaId = 2;

        private readonly List<Translation> testTitles;
        private readonly List<TranslationDTO> testTitleDTOs;
        private readonly Mock<Saga> testSaga;
        private readonly SagaDTO testSagaDTO;
        private readonly Mock<Arc> testArc;
        private readonly ArcDTO testArcDTO;

        public ArcShould()
        {
            var translationDTO = new TranslationDTO { ParentId = null, LanguageCode = "en", Value = "Title" };
            var translation = Translation.CreateFrom(translationDTO);
            testTitles = new List<Translation> { translation };
            testTitleDTOs = new List<TranslationDTO> { translationDTO };

            testSagaDTO = new SagaDTO { Id = testSagaId, Titles = testTitleDTOs };
            testSaga = new Mock<Saga>();
            testSaga.SetupGet(x => x.Id).Returns(testSagaId);
            testSaga.SetupGet(x => x.Titles).Returns(testTitles);

            testArc = new Mock<Arc>();
            testArc.SetupGet(x => x.Id).Returns(testArcId);
            testArc.SetupGet(x => x.FillerType).Returns(testFillerType);
            testArc.SetupGet(x => x.Titles).Returns(testTitles);
            testArc.SetupGet(x => x.Saga).Returns(testSaga.Object);
            testArc.SetupGet(x => x.SagaId).Returns(testSagaId);

            testArcDTO = new ArcDTO
            {
                Id = testArcId,
                FillerType = testFillerType,
                Titles = testTitleDTOs,
                SagaId = testSagaId,
                Saga = testSagaDTO
            };
        }

        [Test]
        public void Return_the_entity_snapshot()
        {
            var snapshot = testArc.Object.GetSnapshot();
            snapshot.Should().BeEquivalentTo(testArcDTO);
        }

        [Test]
        public void Be_created_from_a_snapshot()
        {
            var arc = Arc.CreateFrom(testArcDTO);
            arc.Should().BeEquivalentTo(testArc.Object);
        }

        [Test]
        public void Assign_saga()
        {
            var arc = new Arc();
            var saga = Saga.CreateFrom(testSagaDTO);
            arc.AssignSaga(saga);
            arc.Saga.Should().Be(saga);
            arc.GetSnapshot().SagaId.Should().Be(saga.Id);
        }

        [Test]
        public void Throw_when_assigning_null_saga()
        {
            var arc = new Arc();
            Action act = () => arc.AssignSaga(null!);
            act.Should().Throw<ArgumentNullException>();
        }

    }
}
