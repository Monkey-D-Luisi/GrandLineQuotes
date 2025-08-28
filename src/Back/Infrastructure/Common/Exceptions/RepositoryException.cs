using Application.Common.Exceptions;

namespace Infrastructure.Common.Exceptions
{
    internal class RepositoryException : InfrastructureException
    {


        public RepositoryException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
