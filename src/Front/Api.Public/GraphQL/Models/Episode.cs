namespace Api.Public.GraphQL.Models
{
    public class Episode
    {


        public virtual int Number { get; set; }
        public virtual string? Title { get; set; }
        public virtual int? ArcId { get; set; }
        public Arc? Arc { get; set; }
    }
}
