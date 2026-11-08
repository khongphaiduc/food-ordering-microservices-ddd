using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using food_service.ProductService.Application.Interface;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace food_service.ProductService.Infrastructure.R2
{
    // Image storage backed by Cloudflare R2 (S3-compatible API).
    // Class name kept for compatibility with existing DI registrations.
    public class R2Food : IR2Food
    {
        private const string DefaultImage =
            "https://i.pinimg.com/236x/8b/cf/15/8bcf15e8af97cbd56ab29f15e01933aa.jpg";

        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _s3;
        private readonly ILogger<R2Food> _logger;
        private readonly string _bucket;
        private readonly string _publicBaseUrl;

        public R2Food(
            IConfiguration configuration,
            IAmazonS3 s3Client,
            ILogger<R2Food> logger)
        {
            _configuration = configuration;
            _s3 = s3Client;
            _logger = logger;

            _bucket = _configuration["R2_BUCKET"]
                ?? throw new InvalidOperationException("R2_BUCKET is not configured.");

            _publicBaseUrl = (_configuration["R2_PUBLIC_URL"]
                ?? throw new InvalidOperationException("R2_PUBLIC_URL is not configured."))
                .TrimEnd('/');
        }

        public static IAmazonS3 CreateClient(IConfiguration configuration)
        {
            var endpoint = configuration["R2_ENDPOINT"]
                ?? throw new InvalidOperationException("R2_ENDPOINT is not configured.");
            var accessKey = configuration["R2_ACCESS_KEY_ID"]
                ?? throw new InvalidOperationException("R2_ACCESS_KEY_ID is not configured.");
            var secretKey = configuration["R2_SECRET_ACCESS_KEY"]
                ?? throw new InvalidOperationException("R2_SECRET_ACCESS_KEY is not configured.");

            return new AmazonS3Client(
                new BasicAWSCredentials(accessKey, secretKey),
                new AmazonS3Config
                {
                    ServiceURL = endpoint,
                    AuthenticationRegion = "auto",
                    ForcePathStyle = true,
                    // R2 does not support the newer default flexible checksums.
                    RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                    ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
                });
        }

        public async Task DeleteAsync(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
                return;

            await _s3.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _bucket,
                Key = objectName
            });
        }

        public async Task<string> GetUrlImage(string bucket, string imageName)
        {
            if (string.IsNullOrWhiteSpace(bucket) ||
                string.IsNullOrWhiteSpace(imageName))
            {
                return DefaultImage;
            }

            try
            {
                // Confirm the object exists before handing out a URL.
                await _s3.GetObjectMetadataAsync(_bucket, imageName);

                // Bucket is served through the public R2 custom domain.
                return $"{_publicBaseUrl}/{Uri.EscapeDataString(imageName)}";
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning(
                    "Object not found in R2. Bucket: {Bucket}, Object: {ObjectName}. Message: {Message}",
                    _bucket,
                    imageName,
                    ex.Message);

                return DefaultImage;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error retrieving image URL. Bucket: {Bucket}, Object: {ObjectName}",
                    _bucket,
                    imageName);

                return DefaultImage;
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

            using var inputStream = file.OpenReadStream();

            using var image =
                await Image.LoadAsync(inputStream);

            var originalWidth = image.Width;
            var originalHeight = image.Height;
            var originalSize = file.Length;

            const int maxWidth = 1200;
            const int maxHeight = 1200;

            if (image.Width > maxWidth ||
                image.Height > maxHeight)
            {
                var ratio = Math.Min(
                    (double)maxWidth / image.Width,
                    (double)maxHeight / image.Height);

                var newWidth =
                    (int)(image.Width * ratio);

                var newHeight =
                    (int)(image.Height * ratio);

                image.Mutate(x =>
                    x.Resize(newWidth, newHeight));
            }

            using var outputStream =
                new MemoryStream();

            var encoder = new WebpEncoder
            {
                Quality = 80
            };

            await image.SaveAsync(
                outputStream,
                encoder);

            outputStream.Position = 0;

            var objectName =
                $"{Guid.NewGuid()}.webp";

            // R2 buckets are created in the Cloudflare dashboard, so no bucket creation here.
            await _s3.PutObjectAsync(new PutObjectRequest
            {
                BucketName = _bucket,
                Key = objectName,
                InputStream = outputStream,
                ContentType = "image/webp",
                AutoCloseStream = false,
                // R2 does not support the aws-chunked streaming signature
                // (STREAMING-AWS4-HMAC-SHA256-PAYLOAD) that the SDK uses by default.
                UseChunkEncoding = false
            });

            _logger.LogInformation(
                "Successfully uploaded optimized image. " +
                "ObjectName={ObjectName}, " +
                "Bucket={Bucket}, " +
                "OriginalSize={OriginalSize} bytes, " +
                "OptimizedSize={OptimizedSize} bytes, " +
                "OriginalResolution={OriginalWidth}x{OriginalHeight}, " +
                "FinalResolution={FinalWidth}x{FinalHeight}",
                objectName,
                _bucket,
                originalSize,
                outputStream.Length,
                originalWidth,
                originalHeight,
                image.Width,
                image.Height);

            return objectName;
        }
    }
}
