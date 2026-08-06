namespace food_service.ProductService.Application.Service;

public interface IInventoryDateProvider
{
    DateOnly Today { get; }
}
