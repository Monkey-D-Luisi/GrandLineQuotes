using Application.Contexts.Characters.Queries;
using Domain.Model.Characters;
using Domain.Model.Characters.Abstractions;
using GrandLineQuotes.Client.Abstractions.DTOs.Characters;
using MediatR;

namespace Infrastructure.Contexts.Characters.QueryHandlers
{
    internal class ListCharactersQueryHandler : IRequestHandler<ListCharactersQuery, IEnumerable<CharacterDTO>>
    {


        private readonly ICharacterRepository repository;


        public ListCharactersQueryHandler(ICharacterRepository repository)
        {
            this.repository = repository;
        }


        public async Task<IEnumerable<CharacterDTO>> Handle(ListCharactersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Character> quotesList = await repository.List(cancellationToken);

            return quotesList?.Select(quote => quote.GetSnapshot()) ?? new List<CharacterDTO>();
        }
    }
}
