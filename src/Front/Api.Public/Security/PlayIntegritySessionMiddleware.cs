using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Api.Public.Security
{
    public class PlayIntegritySessionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSessionService _jwt;
        private readonly ILogger<PlayIntegritySessionMiddleware> logger;

        public PlayIntegritySessionMiddleware(RequestDelegate next, JwtSessionService jwt, ILogger<PlayIntegritySessionMiddleware> logger)
        {
            _next = next;
            _jwt = jwt;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/integrity/exchange") ||
                context.Request.Path.StartsWithSegments("/health"))
            {
                await _next(context);
                return;
            }

            var auth = context.Request.Headers["Authorization"].FirstOrDefault();
            if (auth?.StartsWith("Bearer ") == true)
            {
                var token = auth.Substring("Bearer ".Length);
                if (_jwt.Validate(token))
                {
                    await _next(context);
                    return;
                }
            }

            logger.LogWarning("Unauthorized request to {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}
