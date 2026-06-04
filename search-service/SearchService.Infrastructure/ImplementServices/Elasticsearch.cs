using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Nodes;
using Microsoft.Extensions.Caching.Distributed;
using search_service.SearchService.Application.IndexModel;
using search_service.SearchService.Application.Interface;
using System.Text.Json;

namespace search_service.SearchService.Infrastructure.ImplementServices
{
    public class Elasticsearch : IElasticsearch
    {
        private readonly ElasticsearchClient _elasticsearchClient;
        private readonly IDistributedCache _distributeCache;
        private readonly ILogger<Elasticsearch> _logger;

        public Elasticsearch(ElasticsearchClient elasticsearchClient, IDistributedCache distributedCache, ILogger<Elasticsearch> logger)
        {
            _elasticsearchClient = elasticsearchClient;
            _distributeCache = distributedCache;
            _logger = logger;
        }

        public async Task<bool> AddNewProduct(ProductDoc product)
        {
            var result = await _elasticsearchClient.IndexAsync(product, s => s.Index("menu").Id(product.Id));

            return result.IsValidResponse;
        }

        public async Task<bool> DeleteProduct(ProductDoc product)
        {
            var response = await _elasticsearchClient.DeleteAsync<ProductDoc>(product.Id.ToString(), d => d.Index("menu"));
            return response.IsValidResponse;
        }

        public async Task<ProductDoc?> GetProductById(Guid id)
        {
            var response = await _elasticsearchClient.GetAsync<ProductDoc>(
                id.ToString(),
                g => g.Index("menu")
            );

            if (!response.Found) return null;
            return response.Source;
        }


        public async Task<List<ProductDoc>> SearchByKey(string key, int indexPage)
        {
            var cacheKey = $"search:{key}:page:{indexPage}";

            var dataProduct = await _distributeCache.GetStringAsync(cacheKey);  // d? li?u du?c luu vào trong redis là json nên c?n chuy?n v? object 

            if (dataProduct != null)
            {
                var finalContent = JsonSerializer.Deserialize<List<ProductDoc>>(dataProduct);

                return finalContent;
            }

            var size = 10;
            var number = (indexPage - 1) * 10;

            var result = await _elasticsearchClient.SearchAsync<ProductDoc>(s => s.Index("menu").From(number).Size(size)
               .Query(q => q
               .QueryString(qs => qs
                  .Query(key)
                ))
            );


            var productDocument = result.Documents.ToList();

            if (productDocument == null)
            {
                _logger.LogError("Not data from elasticsearch");
            }

            var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(50));
            var contents = JsonSerializer.Serialize(productDocument);

            await _distributeCache.SetStringAsync(cacheKey, contents, option);

            return productDocument;
        }

        public async Task UpdateProduct(ProductDoc product)
        {
            await _elasticsearchClient.IndexAsync(product, s => s.Index("menu").Id(product.Id));
        }
    }
}
