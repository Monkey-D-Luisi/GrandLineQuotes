using Domain.Model.Arcs;
using Domain.Model.Translations;
using GrandLineQuotes.Client.Abstractions.DTOs.Episodes;
using System;
using System.Collections.Generic;

namespace Domain.Model.Episodes
{
    public class Episode
    {


        public virtual int Number { get; private set; }
        public virtual ICollection<Translation>? Titles { get; private set; }
        public virtual int? ArcId { get; private set; }
        public virtual Arc? Arc { get; private set; }


        public Episode()
        {

        }


        private Episode(EpisodeDTO snapshot)
        {
            ApplySnapshot(snapshot);
        }


        public static Episode CreateFrom(EpisodeDTO snapshot)
        {
            return new Episode(snapshot);
        }


        public void UpdateFrom(EpisodeDTO episodeDTO)
        {
            ApplySnapshot(episodeDTO);
        }


        public EpisodeDTO GetSnapshot()
        {
            return new EpisodeDTO
            {
                Number = Number,
                Titles = Titles is not null
                    ? Titles
                        .Select(translation => translation.GetSnapshot())
                        .ToList()
                    : null,
                ArcId = Arc?.Id ?? ArcId,
                Arc = Arc is not null
                    ? Arc.GetSnapshot()
                    : null
            };
        }


        private void ApplySnapshot(EpisodeDTO snapshot)
        {
            Number = snapshot.Number;
            ArcId = snapshot.Arc?.Id ?? snapshot.ArcId ?? ArcId;

            if (snapshot.Titles is not null)
                Titles = snapshot.Titles
                    .Select(title => Translation.CreateFrom(title))
                    .ToList();

            if (snapshot.Arc is not null)
                AssignArc(Arc.CreateFrom(snapshot.Arc));
        }

        public void AssignArc(Arc arc)
        {
            if (arc is null)
                throw new ArgumentNullException(nameof(arc));
            Arc = arc;
        }
    }
}
