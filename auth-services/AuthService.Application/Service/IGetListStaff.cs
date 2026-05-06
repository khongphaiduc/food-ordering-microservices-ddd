using auth_services.AuthService.Application.DTOS;

namespace auth_services.AuthService.Application.Service
{
    public interface IGetListStaff
    {
        Task<List<ViewListStaffDTO>> Execute(CancellationToken token = default);

        Task<List<RoleDTO>> GetListRole(CancellationToken token = default);
    }
}
