using Elastic.Clients.Elasticsearch.TransformManagement;
using Minio.DataModel.Result;

namespace food_service.ProductService.Application.DTOs.Request
{
    public class UpdateProductDTO
    {
        public Guid IdProduct { get; set; }

        public string? Name { get; set; }

        public decimal? Price { get; set; }

        public string? Description { get; set; }

        public bool? IsValiable { get; set; }

        public bool? IsDelete { get; set; }

        public List<ImagesDTO>? AddnewImagesProducts { get; set; }

        public ImagesDTO? AddMainImage { get; set; }

        public List<VariantDTO>? AddNewVariantDTOs { get; set; }

        public List<VariantDTO>? UpdateVariant { get; set; }

        public List<Guid>? DeleteImage { get; set; }

        public List<Guid>? DeleteVariant { get; set; }

    }
    public class ImagesDTO
    {
        public Guid? Id { get; set; }

        public IFormFile images { get; set; }

        public bool IsMain { get; set; } = false;
    }

    public class VariantDTO
    {
        public Guid? IdVariant { get; set; }

        public string Name { get; set; }

        public decimal ExtraPrice { get; set; }

        public bool IsMain { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; } = DateTime.Now;

    }
}
