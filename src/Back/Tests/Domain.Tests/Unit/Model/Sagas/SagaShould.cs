using Domain.Model.Sagas;
using Domain.Model.Translations;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using GrandLineQuotes.Client.Abstractions.DTOs.Sagas;
using Moq;
using Tests.Core;
using System;

namespace Domain.Tests.Unit.Model.Sagas
{
    [TestFixture(Category = "Unit")]
    internal class SagaShould : TestBase
    {
        private const int testSagaId = 1;
        private readonly List<Translation> testTitles;
        private readonly List<TranslationDTO> testTitleDTOs;
        private readonly Mock<Saga> testSaga;
        private readonly SagaDTO testSagaDTO;

        public SagaShould()
        {
            var translationDTO = new TranslationDTO { ParentId = null, LanguageCode = "en", Value = "Title" };
            var translation = Translation.CreateFrom(translationDTO);

            testTitles = new List<Translation> { translation };
            testTitleDTOs = new List<TranslationDTO> { translationDTO };

            testSaga = new Mock<Saga>();
            testSaga.SetupGet(x => x.Id).Returns(testSagaId);
            testSaga.SetupGet(x => x.Titles).Returns(testTitles);

            testSagaDTO = new SagaDTO
            {
                Id = testSagaId,
                Titles = testTitleDTOs
            };
        }

        [Test]
        public void Return_the_entity_snapshot()
        {
            var snapshot = testSaga.Object.GetSnapshot();
            snapshot.Should().BeEquivalentTo(testSagaDTO);
        }

        [Test]
        public void Be_created_from_a_snapshot()
        {
            var saga = Saga.CreateFrom(testSagaDTO);
            saga.Should().BeEquivalentTo(testSaga.Object);
        }

    }
}
