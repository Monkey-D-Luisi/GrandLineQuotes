using Application.Common.IoC;
using Application.Contexts.Quotes.Services;
using Infrastructure.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Infrastructure.Contexts.Quotes.Services
{
    [Ioc(typeof(IVideoService))]
    internal class VideoService : IVideoService
    {


        /// <summary>Configuration key for the Minio bucket that stores quote videos.</summary>
        private const string MinioBucketConfigKey = "Minio:Bucket";

        /// <summary>Configuration key for the path inside the Minio bucket where quote videos are stored.</summary>
        private const string MinioQuoteVideoPathConfigKey = "Minio:QuoteVideoPath";

        private readonly string bucketName;
        private readonly string path;

        private readonly IMinioClient minioClient;
        private readonly ILogger<VideoService> logger;

        public VideoService(IMinioClient minioClient, IConfiguration configuration, ILogger<VideoService> logger)
        {
            this.minioClient = minioClient;
            this.logger = logger;
            bucketName = configuration.GetValue<string?>(MinioBucketConfigKey) ?? "quotes";
            path = configuration.GetValue<string?>(MinioQuoteVideoPathConfigKey) ?? "videos";
        }


        public async Task<bool> Exists(string fileName, CancellationToken cancellationToken)
        {
            try
            {
                await minioClient.StatObjectAsync(
                    new StatObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject($"{path}/{fileName}"),
                    cancellationToken);

                return true;
            }
            catch (ObjectNotFoundException)      
            {
                return false;
            }
        }


        public async Task Upload(Stream stream, string fileName, string contentType, CancellationToken cancellationToken)
        {
            try
            {
                bool bucketExists = await minioClient.BucketExistsAsync(
                    new BucketExistsArgs().WithBucket(bucketName),
                    cancellationToken);

                if (!bucketExists)
                {
                    logger.LogError("Bucket {Bucket} does not exist when uploading file {File}", bucketName, fileName);
                    throw new VideoStorageException($"Bucket '{bucketName}' does not exist.");
                }

                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);

                PutObjectArgs putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject($"{path}/{fileName}")
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType(contentType);

                await minioClient.PutObjectAsync(putObjectArgs, cancellationToken);
            }
            catch (VideoStorageException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading file {File} to bucket {Bucket}", fileName, bucketName);
                throw new VideoStorageException("Error uploading video", ex);
            }
        }


        public async Task Delete(string fileName, CancellationToken cancellationToken)
        {
            try
            {
                RemoveObjectArgs removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject($"{path}/{fileName}");

                await minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting file {File} from bucket {Bucket}", fileName, bucketName);
                throw new VideoStorageException("Error deleting video", ex);
            }
        }
    }
}
