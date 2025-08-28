using GrandLineQuotes.Client.Abstractions.DTOs.Episodes;
using MediatR;

namespace Application.Contexts.Episodes.Queries
{
    public class ListEpisodesQuery : IRequest<IEnumerable<EpisodeDTO>>
    {
    }
}
