namespace auth_services.AuthService.Application.Interfaces
{
    public interface ISetupDatbase
    {
        Task<bool> SetupDatabaseAsync(CancellationToken cancellationToken = default);

    }
}
