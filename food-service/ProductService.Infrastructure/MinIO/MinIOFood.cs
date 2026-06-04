using food_service.ProductService.Application.Interface;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace food_service.ProductService.Infrastructure.MinIO
{
    public class MinIOFood : IMinIOFood
    {
        private readonly IConfiguration _configuration;
        private readonly IMinioClient _clientMinIO;

        public MinIOFood(IConfiguration configuration, IMinioClient minioClient)
        {
            _configuration = configuration;
            _clientMinIO = minioClient;
        }


        public async Task DeleteAsync(string objectName)
        {
            await _clientMinIO.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(_configuration["Minio:Bucket"])
                .WithObject(objectName));
        }


        public async Task<string> GetUrlImage(string bucket, string imageName)
        {
            if (string.IsNullOrWhiteSpace(bucket) || string.IsNullOrWhiteSpace(imageName))
                return "https://i.pinimg.com/236x/8b/cf/15/8bcf15e8af97cbd56ab29f15e01933aa.jpg";

            try
            {

                await _clientMinIO.StatObjectAsync(
                new StatObjectArgs()
                        .WithBucket(bucket)
                        .WithObject(imageName)
                );

                var url = await _clientMinIO.PresignedGetObjectAsync(
                    new PresignedGetObjectArgs()
                        .WithBucket(bucket)
                        .WithObject(imageName)
                        .WithExpiry(60 * 60)
                );

                return url;
            }
            catch (Minio.Exceptions.ObjectNotFoundException)
            {
                return "https://i.pinimg.com/236x/8b/cf/15/8bcf15e8af97cbd56ab29f15e01933aa.jpg";
            }
            catch (Exception ex)
            {

                return "https://i.pinimg.com/236x/8b/cf/15/8bcf15e8af97cbd56ab29f15e01933aa.jpg";
            }
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var objectName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";   //GetExtention l?y duôi file

            using var stream = file.OpenReadStream();

            var found = await _clientMinIO.BucketExistsAsync(new BucketExistsArgs().WithBucket(_configuration["Minio:Bucket"]));

            if (!found)
            {
                await _clientMinIO.MakeBucketAsync(new MakeBucketArgs().WithBucket(_configuration["Minio:Bucket"]));
            }

            await _clientMinIO.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(_configuration["Minio:Bucket"])
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType(file.ContentType)
            );
            return objectName;
        }
    }
}
