using auth_services.AuthService.Domain.Aggregate;
using System.Security;

namespace auth_services.AuthService.Domain.Interface
{
    public interface IAddSafttRepository
    {
        Task<bool> AddSafttAsync(UserAggregate userAggregate, Guid IDRole);
    }
}
