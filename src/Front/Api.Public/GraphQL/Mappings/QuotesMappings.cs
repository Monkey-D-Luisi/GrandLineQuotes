using Api.Public.GraphQL.Models;
using AutoMapper;
using GrandLineQuotes.Client.Abstractions.DTOs.Arcs;
using GrandLineQuotes.Client.Abstractions.DTOs.Characters;
using GrandLineQuotes.Client.Abstractions.DTOs.Episodes;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using GrandLineQuotes.Client.Abstractions.DTOs.Sagas;
using System.Globalization;

namespace Api.Public.GraphQL.Mappings
{
    public class QuotesMappings : Profile
    {


        public QuotesMappings()
        {
            CreateMap<QuoteDTO, Quote>()
                .ForMember(dest => dest.Translation, src => src.MapFrom(p => MapQuoteTranslation(p)));
            CreateMap<CharacterDTO, Character>()
                .ForMember(dest => dest.Name, src => src.MapFrom(p => p.Alias ?? p.Name));
            CreateMap<EpisodeDTO, Episode>()
                .ForMember(dest => dest.Title, src => src.MapFrom(p => MapEpisodeTitle(p)));
            CreateMap<ArcDTO, Arc>()
                .ForMember(dest => dest.Title, src => src.MapFrom(p => MapArcTitle(p)));
            CreateMap<SagaDTO, Saga>()
                .ForMember(dest => dest.Title, src => src.MapFrom(p => MapSagaTitle(p)));
            CreateMap<VideoDTO, Video>();
        }


        private static string? MapQuoteTranslation(QuoteDTO p)
        {
            return p.Translations.FirstOrDefault(translation => translation.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)?.Value ?? p.Translations.First(translation => translation.LanguageCode == "en").Value;
        }


        private static string? MapEpisodeTitle(EpisodeDTO p)
        {
            return p.Titles.FirstOrDefault(title => title.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)?.Value ?? p.Titles.First(title => title.LanguageCode == "en").Value;
        }


        private static string? MapArcTitle(ArcDTO p)
        {
            return p.Titles.FirstOrDefault(title => title.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)?.Value ?? p.Titles.First(title => title.LanguageCode == "en").Value;
        }


        private static string? MapSagaTitle(SagaDTO p)
        {
            return p.Titles.FirstOrDefault(title => title.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)?.Value ?? p.Titles.First(title => title.LanguageCode == "en").Value;
        }
    }
}
