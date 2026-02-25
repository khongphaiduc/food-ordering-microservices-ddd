using cart_service.CartService.Application.DTOs;

namespace cart_service.CartService.Application.Services
{
    public interface ICreateNewCart
    {
        Task<Guid> Execute(RequestCreateNewCartUser request); // id cart

    }
}
