namespace food_service.ProductService.Application.DTOs.Request
{
    public class RequestGetListProduct
    {
        public string? Keyword { get; set; }          

        public Guid? CategoryId { get; set; }        

        public int PageIndex { get; set; } = 1;       
        public int PageSize { get; set; } = 12;        
    }
}
