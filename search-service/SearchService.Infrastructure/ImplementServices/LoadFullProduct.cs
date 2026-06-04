using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using search_service.Models;
using search_service.SearchService.Application.IndexModel;
using search_service.SearchService.Application.Interface;

namespace search_service.SearchService.Infrastructure.ImplementServices
{
    public class LoadFullProduct : ILoadFullProduct
    {
        private readonly FoodProductsDbContext _db;
        private readonly ElasticsearchClient _clientElasticsearch;

        public LoadFullProduct(FoodProductsDbContext foodProductsDbContext, ElasticsearchClient elasticsearchClient)
        {
            _db = foodProductsDbContext;
            _clientElasticsearch = elasticsearchClient;
        }

        public async Task<bool> LoadFullProductAsync()
        {

            var listProduct = _db.Products
                .Select(s => new ProductDoc
                {
                    Id = s.Id,
                    Name = s.Name,
                    Price = s.Price,
                    Description = s.Description,
                    IdCategory = s.CategoryId,
                    CreateAt = s.CreatedAt,
                    UpdateAt = s.UpdatedAt,
                    productImageInternalDTOs = s.ProductImages.Select(t => new ImageDoc
                    {
                        Id = t.Id,
                        UrlImage = t.ImageUrl,
                        IsMain = t.IsMain
                    }).ToList()
                })
                .ToList();

            if (!listProduct.Any())
                return true;

            // T?o BulkOperationsCollection
            var operations = new BulkOperationsCollection();

            foreach (var p in listProduct)
            {
                operations.Add(new BulkIndexOperation<ProductDoc>(p)
                {
                    Id = p.Id.ToString()
                });
            }

            var bulkRequest = new BulkRequest("menu")
            {
                Operations = operations
            };

            // G?i BulkAsync
            var response = await _clientElasticsearch.BulkAsync(bulkRequest);

            // Check l?i
            if (!response.IsValidResponse || response.Errors)
            {

                return false;
            }

            return true;
        }


    }
}
