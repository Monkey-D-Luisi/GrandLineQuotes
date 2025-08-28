namespace GrandLineQuotes.Client.Abstractions.DTOs.Quotes
{
    public class TranslationDTO
    {


        public virtual int? ParentId { get; set; }
        public virtual string? LanguageCode { get; set; }
        public virtual string? Value { get; set; }
    }
}
