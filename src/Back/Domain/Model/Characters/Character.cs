using GrandLineQuotes.Client.Abstractions.DTOs.Characters;

using System;
namespace Domain.Model.Characters
{
    public class Character
    {


        public virtual int Id { get; private set; }
        public virtual string? Name { get; private set; }
        public virtual string? Alias { get; private set; }


        public Character()
        {
            
        }


        private Character(CharacterDTO snapshot)
        {
            ApplySnapshot(snapshot);
        }


        public static Character CreateFrom(CharacterDTO snapshot)
        {
            return new Character(snapshot);
        }


        public void UpdateFrom(CharacterDTO snapshot)
        {
            ApplySnapshot(snapshot);
        }


        public CharacterDTO GetSnapshot()
        {
            return new CharacterDTO
            {
                Id = Id,
                Name = Name,
                Alias = Alias,
            };
        }


        private void ApplySnapshot(CharacterDTO snapshot)
        {
            Id = snapshot.Id ?? Id;
            if (snapshot.Name is not null)
                Rename(snapshot.Name);
            UpdateAlias(snapshot.Alias);
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.", nameof(name));
            Name = name;
        }

        public void UpdateAlias(string? alias)
        {
            Alias = string.IsNullOrWhiteSpace(alias) ? null : alias;
        }
    }
}
