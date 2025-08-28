namespace GrandLineQuotes.Client.Abstractions.DTOs.Common
{
    /// <summary>
    /// Supported language codes for quote videos.
    /// </summary>
    public static class LanguagePath
    {
        /// <summary>Default language directory.</summary>
        public const string English = "base";

        /// <summary>Spanish language directory.</summary>
        public const string Spanish = "es";

        /// <summary>All supported language codes.</summary>
        public static readonly string[] All = { English, Spanish };
    }
}
