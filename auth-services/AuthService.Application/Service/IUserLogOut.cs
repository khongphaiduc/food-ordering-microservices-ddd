namespace auth_services.AuthService.Application.Service
{
    public interface IUserLogOut
    {
        Task<bool> Execute(Guid userId);
    }
}
