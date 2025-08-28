using Domain.Model.Characters;
using Domain.Model.Episodes;
using Domain.Model.Translations;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using System;

namespace Domain.Model.Quotes
{
    public class Quote
    {


        public virtual int Id { get; private set; }
        public virtual string? OriginalText { get; private set; }
        public virtual string? Text { get; private set; }
        public virtual ICollection<Translation>? Translations { get; private set; }
        public virtual int? AuthorId { get; private set; }
        public virtual Character? Author { get; private set; }
        public virtual int? EpisodeNumber { get; private set; }
        public virtual Episode? Episode { get; private set; }
        public virtual bool IsReviewed { get; private set; }


        public Quote()
        {

        }


        private Quote(QuoteDTO snapshot)
        {
            ApplySnapshot(snapshot);
        }


        public static Quote CreateFrom(QuoteDTO snapshot)
        {
            return new Quote(snapshot);
        }


        public void UpdateFrom(QuoteDTO quoteDTO)
        {
            ApplySnapshot(quoteDTO);
        }


        public QuoteDTO GetSnapshot()
        {
            return new QuoteDTO
            {
                Id = Id,
                OriginalText = OriginalText,
                Text = Text,
                Translations = Translations is not null
                    ? Translations
                        .Select(translation => translation.GetSnapshot())
                        .ToList()
                    : null,
                AuthorId = Author?.Id ?? AuthorId,
                Author = Author is not null
                    ? Author.GetSnapshot()
                    : null,
                EpisodeNumber = EpisodeNumber,
                Episode = Episode is not null
                    ? Episode.GetSnapshot()
                    : null,
                IsReviewed = IsReviewed
            };
        }


        private void ApplySnapshot(QuoteDTO snapshot)
        {
            Id = snapshot.Id ?? Id;
            if (snapshot.OriginalText is not null)
                SetOriginalText(snapshot.OriginalText);
            if (snapshot.Text is not null)
                UpdateText(snapshot.Text);
            AuthorId = snapshot.Author?.Id ?? snapshot.AuthorId ?? AuthorId;
            EpisodeNumber = snapshot.Episode?.Number ?? snapshot.EpisodeNumber ?? EpisodeNumber;
            IsReviewed = snapshot.IsReviewed ?? IsReviewed;

            if (snapshot.Translations is not null)
                Translations = snapshot.Translations
                    .Select(translation => Translation.CreateFrom(translation))
                    .ToList();

            if (snapshot.Author is not null)
                AssignAuthor(Character.CreateFrom(snapshot.Author));

            if (snapshot.Episode is not null)
                Episode = Episode.CreateFrom(snapshot.Episode);
        }

        public void AssignAuthor(Character author)
        {
            if (author is null)
                throw new ArgumentNullException(nameof(author));
            Author = author;
        }
    
        public void Review()
        {
            if (IsReviewed)
                throw new InvalidOperationException("Quote already reviewed.");
            if (string.IsNullOrWhiteSpace(OriginalText))
                throw new InvalidOperationException("Quote must have original text before review.");
            IsReviewed = true;
        }

        public void SetOriginalText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Original text cannot be empty.", nameof(text));
            OriginalText = text;
        }

        public void UpdateText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty.", nameof(text));
            Text = text;
        }
    }
}
