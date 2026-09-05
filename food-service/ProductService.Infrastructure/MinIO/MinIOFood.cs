using food_service.ProductService.Application.Interface;
using Minio;
using Minio.DataModel.Args;

namespace food_service.ProductService.Infrastructure.MinIO
{
    public class MinIOFood : IMinIOFood
    {
        private readonly IConfiguration _configuration;
        private readonly IMinioClient _clientMinIO;

        private readonly IMinioClient _clientMinIOPublic;

        private readonly ILogger<MinIOFood> _logger;

        public MinIOFood(
            IConfiguration configuration,
            IMinioClient minioClient,
            ILogger<MinIOFood> logger)
        {
            _configuration = configuration;
            _clientMinIO = minioClient;
            _logger = logger;

  

            var publicEndpoint =
                _configuration["PublicEndpoint"]
                ?? throw new InvalidOperationException(
                    "PublicEndpoint is not configured.");

            var publicUseSSL =
                _configuration.GetValue<bool>("PublicUseSSL");

            _clientMinIOPublic = new MinioClient()
                .WithEndpoint(publicEndpoint)
                .WithCredentials(
                    _configuration["MinIOAccessKey"],
                    _configuration["MinIOSecretKey"])
                .WithSSL(publicUseSSL)
                .Build();
        }


        public async Task DeleteAsync(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
                return;

            var bucket = _configuration["MinIOBucket"];

            if (string.IsNullOrWhiteSpace(bucket))
                throw new InvalidOperationException(
                    "MinIOBucket is not configured.");

            await _clientMinIO.RemoveObjectAsync(
                new RemoveObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(objectName)
            );
        }

        public async Task<string> GetUrlImage(
            string bucket,
            string imageName)
        {
            const string defaultImage =
                "https://i.pinimg.com/236x/8b/cf/15/8bcf15e8af97cbd56ab29f15e01933aa.jpg";

            if (string.IsNullOrWhiteSpace(bucket) ||
                string.IsNullOrWhiteSpace(imageName))
            {
                return defaultImage;
            }

            try
            {
  

                await _clientMinIO.StatObjectAsync(
                    new StatObjectArgs()
                        .WithBucket(bucket)
                        .WithObject(imageName)
                );




                var url =
                    await _clientMinIOPublic.PresignedGetObjectAsync(
                        new PresignedGetObjectArgs()
                            .WithBucket(bucket)
                            .WithObject(imageName)
                            .WithExpiry(60 * 60)
                    );

                return url;
            }
            catch (Minio.Exceptions.ObjectNotFoundException ex)
            {
                _logger.LogWarning(
                    "Object not found in MinIO. Bucket: {Bucket}, Object: {ObjectName}. Message: {Message}",
                    bucket,
                    imageName,
                    ex.Message);

                return defaultImage;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error retrieving image URL. Bucket: {Bucket}, Object: {ObjectName}",
                    bucket,
                    imageName);

                return defaultImage;
            }
        }


        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException(
                    "File is empty.",
                    nameof(file));
            }

            var bucket = _configuration["MinIOBucket"];

            if (string.IsNullOrWhiteSpace(bucket))
            {
                throw new InvalidOperationException(
                    "MinIOBucket is not configured.");
            }


            /*
             * Generate tên file mới để tránh trùng object.
             *
             * Ví dụ:
             *
             * original:
             * pizza.jpg
             *
             * object:
             * 8c0c8f3a-4f5e-4b2f-9c12-a123456789.jpg
             */

            var extension =
                Path.GetExtension(file.FileName);

            var objectName =
                $"{Guid.NewGuid()}{extension}";


            using var stream = file.OpenReadStream();


            // =====================================================
            // CHECK BUCKET
            // =====================================================

            var found =
                await _clientMinIO.BucketExistsAsync(
                    new BucketExistsArgs()
                        .WithBucket(bucket)
                );


            if (!found)
            {
                await _clientMinIO.MakeBucketAsync(
                    new MakeBucketArgs()
                        .WithBucket(bucket)
                );
            }


            // =====================================================
            // UPLOAD
            // =====================================================

            await _clientMinIO.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(file.Length)
                    .WithContentType(file.ContentType)
            );


            _logger.LogInformation(
                "Successfully uploaded object {ObjectName} to bucket {Bucket}",
                objectName,
                bucket);


            return objectName;
        }
    }
}