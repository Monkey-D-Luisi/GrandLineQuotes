using Application.Common.Exceptions;

namespace Infrastructure.Common.Exceptions
{
    internal class VideoStorageException : InfrastructureException
    {


        public VideoStorageException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }

}
