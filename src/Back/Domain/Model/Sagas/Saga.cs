using Domain.Model.Translations;
using GrandLineQuotes.Client.Abstractions.DTOs.Sagas;
using System.Collections.Generic;

namespace Domain.Model.Sagas
{
    public class Saga
    {


        public virtual int Id { get; private set; }
        public virtual ICollection<Translation>? Titles { get; private set; }


        public Saga()
        {

        }


        private Saga(SagaDTO snapshot)
        {
            ApplySnapshot(snapshot);
        }


        public static Saga CreateFrom(SagaDTO snapshot)
        {
            return new Saga(snapshot);
        }


        public void UpdateFrom(SagaDTO snapshot)
        {
            ApplySnapshot(snapshot);
        }


        public SagaDTO GetSnapshot()
        {
            return new SagaDTO
            {
                Id = Id,
                Titles = Titles is not null
                    ? Titles
                        .Select(translation => translation.GetSnapshot())
                        .ToList()
                    : null,
            };
        }


        private void ApplySnapshot(SagaDTO snapshot)
        {
            Id = snapshot.Id ?? Id;

            if (snapshot.Titles is not null)
                Titles = snapshot.Titles
                    .Select(title => Translation.CreateFrom(title))
                    .ToList();
        }

    }
}
