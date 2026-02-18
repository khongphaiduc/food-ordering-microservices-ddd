using Elastic.Clients.Elasticsearch.Nodes;
using Microsoft.AspNetCore.Http.Metadata;

namespace food_service.ProductService.Application.DTOs.Request
{
    public class CreateNewProducDTO
    {
        public Guid IdCategory { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public List<ImageProduct>? ImageProduct { get; set; }

        public ImageProduct? MainImage { get; set; }
    }


    public class ImageProduct
    {
        public IFormFile image { get; set; }

        public bool IsMain { get; set; } = false;

    }
}
