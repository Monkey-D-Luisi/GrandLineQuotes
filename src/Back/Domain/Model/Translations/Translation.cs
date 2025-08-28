using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using System;

namespace Domain.Model.Translations
{
    public class Translation
    {


        public virtual int? ParentId { get; private set; }
        public virtual string? LanguageCode { get; private set; }
        public virtual string? Value { get; private set; }


        public Translation()
        {

        }


        private Translation(TranslationDTO snapshot)
        {
            ApplySnapshot(snapshot);
        }


        public static Translation CreateFrom(TranslationDTO snapshot)
        {
            return new Translation(snapshot);
        }


        public void UpdateFrom(TranslationDTO translationDTO)
        {
            ApplySnapshot(translationDTO);
        }


        public TranslationDTO GetSnapshot()
        {
            return new TranslationDTO
            {
                ParentId = ParentId,
                LanguageCode = LanguageCode,
                Value = Value
            };
        }


        private void ApplySnapshot(TranslationDTO snapshot)
        {
            ParentId = snapshot.ParentId ?? ParentId;
            if (snapshot.LanguageCode is not null)
                ChangeLanguage(snapshot.LanguageCode);
            if (snapshot.Value is not null)
                SetValue(snapshot.Value);
        }

        public void ChangeLanguage(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
                throw new ArgumentException("Language code cannot be empty.", nameof(languageCode));
            LanguageCode = languageCode;
        }

        public void SetValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be empty.", nameof(value));
            Value = value;
        }
    }
}
