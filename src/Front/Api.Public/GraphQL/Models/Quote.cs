namespace Api.Public.GraphQL.Models
{
    public class Quote
    {


        public virtual int? Id { get; set; }
        public virtual string? OriginalText { get; set; }
        public virtual string? Text { get; set; }
        public virtual string? Translation { get; set; }
        public virtual int? AuthorId { get; set; }
        public virtual Character? Author { get; set; }
        public virtual int? EpisodeNumber { get; set; }
        public virtual Episode? Episode { get; set; }
        public virtual Video? Video { get; set; }
    }
}
