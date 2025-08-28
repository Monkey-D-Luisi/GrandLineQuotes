
namespace GrandLineQuotes.Client.Abstractions.DTOs.Quotes
{
    public class VideoDTO
    {


        public virtual Stream? Content { get; set; }
        public virtual string? Name { get; set; }
        public virtual string? ContentType { get; set; }
    }
}
