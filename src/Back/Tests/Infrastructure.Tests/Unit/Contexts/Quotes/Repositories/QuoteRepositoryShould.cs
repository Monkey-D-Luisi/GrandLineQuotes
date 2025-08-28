using Domain.Model.Quotes;
using FluentAssertions;
using GrandLineQuotes.Client.Abstractions.DTOs.Quotes;
using Infrastructure.Common.DatabaseContexts;
using Infrastructure.Contexts.Quotes.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.Unit.Contexts.Quotes.Repositories
{
    [TestFixture(Category = "Unit")]
    public class QuoteRepositoryShould
    {
        [Test]
        public async Task Not_throw_when_translation_for_current_culture_missing()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new ApplicationDbContext(options);
            var repository = new QuoteRepository(context);

            var quote = Quote.CreateFrom(new QuoteDTO
            {
                Text = "english text",
                Translations = new List<TranslationDTO>
                {
                    new() { LanguageCode = "es", Value = "hola" }
                }
            });

            await repository.Save(quote, CancellationToken.None);

            var result = await repository.List(null, null, "adis", null, CancellationToken.None);
            result.Should().BeEmpty();
        }
    }
}
