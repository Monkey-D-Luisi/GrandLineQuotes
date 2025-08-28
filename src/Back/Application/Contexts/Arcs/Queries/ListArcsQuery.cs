using GrandLineQuotes.Client.Abstractions.DTOs.Arcs;
using MediatR;

namespace Application.Contexts.Arcs.Queries
{
    public class ListArcsQuery : IRequest<IEnumerable<ArcDTO>>
    {
    }
}
