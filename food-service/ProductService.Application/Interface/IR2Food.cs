namespace food_service.ProductService.Application.Interface
{
    public interface IR2Food
    {
        Task<string> UploadAsync(IFormFile file);
        Task DeleteAsync(string objectName);

        Task<string> GetUrlImage(string bucket,string imageName);
    }
}
