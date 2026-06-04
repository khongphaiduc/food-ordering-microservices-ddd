using auth_services.AuthService.Domain.Aggregate;
using System.Security;

namespace auth_services.AuthService.Domain.Interface
{
    public interface IAddStaffRepository
    {
        Task<bool> AddStaffAsync(UserAggregate userAggregate, Guid IDRole);
    }
}
