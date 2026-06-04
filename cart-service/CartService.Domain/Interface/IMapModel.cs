using cart_service.CartService.Domain.Aggregate;
using cart_service.CartService.Infrastructure.Models;

namespace cart_service.CartService.Domain.Interface
{
    public interface IMapModel
    {
        Cart MapAggregateToCartModel(CartAggregate cartAggregate);



    }
}
