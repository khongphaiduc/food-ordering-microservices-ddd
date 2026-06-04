using Elastic.Clients.Elasticsearch;
using search_service.SearchService.Application.Interface;

namespace search_service.SearchService.Infrastructure.ImplementServices
{
    public class SuggestSearch : ISuggestSearch
    {
        private readonly ElasticsearchClient _elasticsearchClient;

        public SuggestSearch(ElasticsearchClient elasticsearchClient)
        {
            _elasticsearchClient = elasticsearchClient;
        }

        public async Task<List<string>> SuggestSearchAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return new List<string>();

            var response = await _elasticsearchClient.SearchAsync<SearchSuggestion>(s => s
                .Index("menu")
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name) // Tr? dúng vào field "name" trong mapping
                        .Query(name)
                        .Analyzer("autocomplete_search") // Ép s? d?ng analyzer search dã khai báo
                    )
                )
                .Size(10)
            );

            if (!response.IsValidResponse)
            {           
                var debug = response.DebugInformation;
                return new List<string>();
            }

            // Trích xu?t k?t qu?
            return response.Documents
                           .Select(d => d.Name)
                           .Distinct()
                           .ToList();
        }
    }

    public class SearchSuggestion
    {
        public string Name { get; set; }
    }
}
