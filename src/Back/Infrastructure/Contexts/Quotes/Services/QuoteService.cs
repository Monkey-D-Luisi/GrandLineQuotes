using Application.Common.IoC;
using Application.Contexts.Quotes.Services;
using Domain.Model.Characters;
using Domain.Model.Quotes;
using Domain.Model.Quotes.Abstractions;
using GrandLineQuotes.Client.Abstractions.DTOs.Characters;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using Infrastructure.Common.Exceptions;

namespace Infrastructure.Contexts.Quotes.Services
{
    [Ioc(typeof(IQuoteService))]
    internal class QuoteService : IQuoteService
    {


        private readonly IQuoteRepository repository;


        public QuoteService(IQuoteRepository repository)
        {
            this.repository = repository;
        }


        public async Task Save(QuoteDTO quoteDTO, CancellationToken cancellationToken)
        {
            Quote? quote = null;

            try
            {
                if ((quoteDTO.Id ?? 0) > 0)
                    quote = await repository.Get(quoteDTO.Id ?? 0, cancellationToken, tracked: true);

                if (quote is null)
                    quote = Quote.CreateFrom(quoteDTO);
                else
                    quote.UpdateFrom(quoteDTO);

                await repository.Save(quote, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error calling repository", ex);
            }

            quoteDTO.Id = quote.Id;
            if (quoteDTO.Videos is not null)
            {
                foreach (var video in quoteDTO.Videos)
                {
                    if ((video.Value.Content?.Length ?? 0) > 0)
                        video.Value.Name = $"{video.Key}/{quote.Id}.{GetExtensionFromContentType(video.Value?.ContentType)}";
                }
            }
        }


        public async Task Delete(int quoteId, CancellationToken cancellationToken)
        {
            try
            {
                await repository.Delete(quoteId, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error calling repository", ex);
            }
        }


        private static string GetExtensionFromContentType(string? contentType)
        {
            return contentType switch
            {
                "video/mp4" => "mp4",
                _ => throw new NotSupportedException($"Content type '{contentType}' is not supported.")
            };
        }
    }
}
