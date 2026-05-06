using auth_services.AuthService.Application.DTOS;

namespace auth_services.AuthService.Application.Service
{
    public interface IAddAccountStaffs
    {
        Task<bool> AddAccountStaffsAsync(AddAccountStaffDTO accountStaffs, CancellationToken token = default);
    }
}
