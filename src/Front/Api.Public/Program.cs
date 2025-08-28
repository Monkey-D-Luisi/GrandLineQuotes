using Api.Public.GraphQL.Bindings;
using Api.Public.GraphQL.Queries;
using Api.Public.Security;
using Client.Clients.Extensions;
using System.Globalization;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiClient();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = builder.Configuration.GetValue<string>("LuckyPenny:LicenseKey");
},
AppDomain.CurrentDomain.GetAssemblies());

builder.Services.Configure<PlayIntegrityOptions>(
    builder.Configuration.GetSection("PlayIntegrity"));
builder.Services.AddSingleton<JwtSessionService>();
builder.Services.AddSingleton<IPlayIntegrityService, PlayIntegrityService>();

builder.Services.AddMinio(configureClient =>
{
    configureClient
        .WithEndpoint(builder.Configuration.GetValue<string>("Minio:Endpoint"))
        .WithCredentials(
            builder.Configuration.GetValue<string>("Minio:AccessKey"),
            builder.Configuration.GetValue<string>("Minio:SecretKey"))
        .WithSSL(builder.Configuration.GetValue<bool>("Minio:Secure"));
});

builder.Services
    .AddGraphQLServer()
    .DisableIntrospection(false)
    .AddQueryType<QuoteQueries>()
    .AddType<QuoteType>();

var app = builder.Build();

app.UseRouting();

app.Use(async (context, next) =>
{
    var lang = context.Request.Headers["Accept-Language"].FirstOrDefault()?.Split(",").FirstOrDefault() ?? "en";
    try
    {
        var culture = new CultureInfo(lang);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }
    catch
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
    }
    await next.Invoke();
});

app.UseMiddleware<PlayIntegritySessionMiddleware>();

app.MapGraphQL();

app.MapGet("/videos/{language}/{id}.mp4", async (
    string language,
    int id,
    HttpContext context,
    IMinioClient minio,
    IConfiguration configuration) =>
{
    var bucket = configuration.GetValue<string>("Minio:Bucket") ?? "quotes";
    var path = configuration.GetValue<string>("Minio:QuoteVideoPath") ?? "videos";
    var objectName = $"{path}/{language}/{id}.mp4";

    try
    {
        var stat = await minio.StatObjectAsync(
            new StatObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName),
            context.RequestAborted);

        context.Response.Headers["Accept-Ranges"] = "bytes";
        context.Response.Headers["Cache-Control"] = "no-store";
        context.Response.Headers["ETag"] = stat.ETag;
        context.Response.Headers["Last-Modified"] = stat.LastModified.ToString("R");

        var rangeHeader = context.Request.Headers["Range"].ToString();
        long start = 0;
        long length = stat.Size;

        if (!string.IsNullOrEmpty(rangeHeader) && rangeHeader.StartsWith("bytes="))
        {
            var range = rangeHeader[6..].Split('-');
            if (long.TryParse(range[0], out var parsedStart))
            {
                start = parsedStart;
                if (range.Length > 1 && long.TryParse(range[1], out var parsedEnd))
                {
                    length = parsedEnd - start + 1;
                }
                else
                {
                    length = stat.Size - start;
                }
                context.Response.StatusCode = StatusCodes.Status206PartialContent;
                context.Response.Headers["Content-Range"] = $"bytes {start}-{start + length - 1}/{stat.Size}";
            }
        }

        context.Response.ContentType = "video/mp4";
        context.Response.ContentLength = length;

        var getArgs = new GetObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName)
            .WithCallbackStream(async (stream, cancellationToken) =>
            {
                try
                {
                    await stream.CopyToAsync(context.Response.Body, cancellationToken);
                }
                catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
                {
                    // Client disconnected; ignore and stop sending data
                }
                catch (ObjectDisposedException) when (context.RequestAborted.IsCancellationRequested)
                {
                    // Response stream was disposed because the client disconnected
                }
            });

        if (context.Response.StatusCode == StatusCodes.Status206PartialContent)
        {
            getArgs.WithOffsetAndLength(start, length);
        }

        await minio.GetObjectAsync(getArgs, context.RequestAborted);
    }
    catch (ObjectNotFoundException)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
    }
    catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
    {
        // Client cancelled the request; no further action required
    }
});

app.MapPost("/integrity/exchange", async (HttpContext context, IPlayIntegrityService verifier, JwtSessionService jwt, ILogger<Program> logger) =>
{
    var token = context.Request.Headers["X-Play-Integrity-Token"].FirstOrDefault();
    var nonce = context.Request.Headers["X-Play-Integrity-Nonce"].FirstOrDefault();
    var packageName = context.Request.Headers["X-Play-Integrity-Package-Name"].FirstOrDefault();

    if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(nonce) || string.IsNullOrWhiteSpace(packageName))
    {
        logger.LogWarning("Missing Play Integrity headers: Token='{Token}', Nonce='{Nonce}', PackageName='{PackageName}'", token, nonce, packageName);
        return Results.BadRequest();
    }

    if (!await verifier.ValidateAsync(token, nonce, packageName))
    {
        logger.LogWarning("Play Integrity validation failed for PackageName='{PackageName}'", packageName);
        return Results.Unauthorized();
    }

    var session = jwt.CreateToken(nonce);
    return Results.Ok(new { session });
});

// Explicitly expose a basic GET endpoint for health checks
app.MapGet("/health", () => Results.Ok());

app.Run();