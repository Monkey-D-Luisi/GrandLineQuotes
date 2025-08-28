using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Application.Common.Exceptions.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            if (exception.GetType().IsAssignableTo(typeof(InfrastructureException)))
            {
                context.Result = new ObjectResult(new
                {
                    Error = exception.Message
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            else if (exception.GetType().IsAssignableTo(typeof(ArgumentException)))
            {
                var argumentEx = (ArgumentException)exception;
                context.Result = new ObjectResult(new
                {
                    Error = argumentEx.Message,
                    Details = argumentEx.Data
                })
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            else if (exception.GetType().IsAssignableTo(typeof(NotSupportedException)))
            {
                context.Result = new ObjectResult(new
                {
                    Error = exception.Message
                })
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            else
            {
                context.Result = new ObjectResult(new
                {
                    Error = "Unexpected error occurred"
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            logger.LogError(exception, "Unhandled exception");
            context.ExceptionHandled = true;
        }
    }
}

