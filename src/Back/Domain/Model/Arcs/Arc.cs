using Domain.Model.Sagas;
using Domain.Model.Translations;
using GrandLineQuotes.Client.Abstractions.DTOs.Arcs;
using GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums;
using System;
using System.Collections.Generic;

namespace Domain.Model.Arcs
{
    public class Arc
    {


        public virtual int Id { get; private set; }
        public virtual FillerType? FillerType { get; private set; }
        public virtual ICollection<Translation>? Titles { get; private set; }
        public virtual int? SagaId { get; private set; }
        public virtual Saga? Saga { get; private set; }


        public Arc()
        {

        }


        private Arc(ArcDTO snapshot)
        {
            ApplySnapshot(snapshot);
        }


        public static Arc CreateFrom(ArcDTO snapshot)
        {
            return new Arc(snapshot);
        }


        public void UpdateFrom(ArcDTO snapshot)
        {
            ApplySnapshot(snapshot);
        }


        public ArcDTO GetSnapshot()
        {
            return new ArcDTO
            {
                Id = Id,
                FillerType = FillerType,
                Titles = Titles is not null
                    ? Titles
                        .Select(translation => translation.GetSnapshot())
                        .ToList()
                    : null,
                SagaId = Saga?.Id ?? SagaId,
                Saga = Saga is not null
                    ? Saga.GetSnapshot()
                    : null,
            };
        }


        private void ApplySnapshot(ArcDTO snapshot)
        {
            Id = snapshot.Id ?? Id;
            FillerType = snapshot.FillerType;
            SagaId = snapshot.Saga?.Id ?? snapshot.SagaId ?? SagaId;

            if (snapshot.Titles is not null)
                Titles = snapshot.Titles
                    .Select(title => Translation.CreateFrom(title))
                    .ToList();

            if (snapshot.Saga is not null)
            {
                AssignSaga(Saga.CreateFrom(snapshot.Saga));
            }
        }

        public void AssignSaga(Saga saga)
        {
            if (saga is null)
                throw new ArgumentNullException(nameof(saga));
            Saga = saga;
        }
    }
}
