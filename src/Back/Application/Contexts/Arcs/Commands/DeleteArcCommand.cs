using MediatR;

namespace Application.Contexts.Arcs.Commands
{
    public record DeleteArcCommand(int ArcId) : IRequest;
}
