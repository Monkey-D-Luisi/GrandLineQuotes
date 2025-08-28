using GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums;

namespace Api.Public.GraphQL.Models
{
    public class Arc
    {


        public virtual int? Id { get; set; }
        public virtual FillerType? FillerType { get; set; }
        public virtual string? Title { get; set; }
        public virtual int? SagaId { get; set; }
        public virtual Saga? Saga { get; set; }
    }
}
