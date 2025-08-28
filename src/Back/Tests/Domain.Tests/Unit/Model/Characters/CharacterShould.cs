using Domain.Model.Characters;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Characters;
using Tests.Core;
using System;

namespace Domain.Tests.Unit.Model.Characters
{
    [TestFixture(Category = "Unit")]
    internal class CharacterShould : TestBase
    {
        private const int testCharacterId = 1;
        private const string testCharacterName = "TestName";
        private const string testCharacterAlias = "AliasName";

        private readonly Character character;
        private readonly CharacterDTO characterDTO;

        public CharacterShould()
        {
            characterDTO = new CharacterDTO
            {
                Id = testCharacterId,
                Name = testCharacterName,
                Alias = testCharacterAlias
            };

            character = new Character();
            character.UpdateFrom(characterDTO);
        }

        [Test]
        public void Return_the_entity_snapshot()
        {
            var snapshot = character.GetSnapshot();
            snapshot.Should().BeEquivalentTo(characterDTO);
        }

        [Test]
        public void Be_created_from_a_snapshot()
        {
            var result = Character.CreateFrom(characterDTO);
            result.Should().BeEquivalentTo(character);
        }

        [Test]
        public void Rename_character()
        {
            var newCharacter = Character.CreateFrom(characterDTO);
            const string newName = "NewName";
            newCharacter.Rename(newName);
            newCharacter.Name.Should().Be(newName);
        }

        [Test]
        public void Throw_when_name_is_empty()
        {
            var newCharacter = Character.CreateFrom(characterDTO);
            Action act = () => newCharacter.Rename("");
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Update_alias()
        {
            var newCharacter = Character.CreateFrom(characterDTO);
            const string newAlias = "NewAlias";
            newCharacter.UpdateAlias(newAlias);
            newCharacter.Alias.Should().Be(newAlias);
        }

        [Test]
        public void Allow_empty_alias()
        {
            var newCharacter = Character.CreateFrom(characterDTO);
            newCharacter.UpdateAlias(null);
            newCharacter.Alias.Should().BeNull();
            newCharacter.UpdateAlias("");
            newCharacter.Alias.Should().BeNull();
        }

        [Test]
        public void Remove_alias_from_snapshot()
        {
            var newCharacter = Character.CreateFrom(characterDTO);
            var dto = new CharacterDTO { Alias = null };
            newCharacter.UpdateFrom(dto);
            newCharacter.Alias.Should().BeNull();
        }
    }
}
