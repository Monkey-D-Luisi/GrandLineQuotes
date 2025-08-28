using Domain.Model.Translations;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using Tests.Core;
using System;

namespace Domain.Tests.Unit.Model.Translations
{
    [TestFixture(Category = "Unit")]
    internal class TranslationShould : TestBase
    {
        private const int testParentId = 1;
        private const string testLanguageCode = "en";
        private const string testValue = "Value";

        private readonly Translation translation;
        private readonly TranslationDTO translationDTO;

        public TranslationShould()
        {
            translationDTO = new TranslationDTO
            {
                ParentId = testParentId,
                LanguageCode = testLanguageCode,
                Value = testValue
            };

            translation = Translation.CreateFrom(translationDTO);
        }

        [Test]
        public void Return_the_entity_snapshot()
        {
            var snapshot = translation.GetSnapshot();
            snapshot.Should().BeEquivalentTo(translationDTO);
        }

        [Test]
        public void Be_created_from_a_snapshot()
        {
            var result = Translation.CreateFrom(translationDTO);
            result.Should().BeEquivalentTo(translation);
        }

        [Test]
        public void Change_language_code()
        {
            var t = Translation.CreateFrom(translationDTO);
            t.ChangeLanguage("es");
            t.LanguageCode.Should().Be("es");
        }

        [Test]
        public void Throw_when_language_code_is_empty()
        {
            var t = Translation.CreateFrom(translationDTO);
            Action act = () => t.ChangeLanguage("");
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Set_value()
        {
            var t = Translation.CreateFrom(translationDTO);
            const string newValue = "NewValue";
            t.SetValue(newValue);
            t.Value.Should().Be(newValue);
        }

        [Test]
        public void Throw_when_value_is_empty()
        {
            var t = Translation.CreateFrom(translationDTO);
            Action act = () => t.SetValue("");
            act.Should().Throw<ArgumentException>();
        }
    }
}
