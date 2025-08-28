namespace Api.Public.GraphQL.Models
{
    public class Video
    {


        public virtual Stream? Content { get; set; }
        public virtual string? Name { get; set; }
        public virtual string? ContentType { get; set; }
    }
}
