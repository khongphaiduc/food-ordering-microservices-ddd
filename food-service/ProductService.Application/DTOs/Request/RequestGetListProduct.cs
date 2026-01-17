namespace food_service.ProductService.Application.DTOs.Request
{
    public class RequestGetListProduct
    {
        public string? Keyword { get; set; }          // tìm theo tên sản phẩm

        public Guid? CategoryId { get; set; }          // lọc theo category

        public int PageIndex { get; set; } = 1;        // trang hiện tại (bắt đầu từ 1)
        public int PageSize { get; set; } = 10;        // số item / trang
    }
}
