using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Infastructure.DbContextAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace auth_services.AuthService.Infastructure.ServiceImpelemt
{

    public class GetListStaff : IGetListStaff
    {
        private readonly FoodAuthContext _db;

        public GetListStaff(FoodAuthContext foodAuthContext)
        {
            _db = foodAuthContext;
        }


        public async Task<List<ViewListStaffDTO>> Execute()
        {
            var listStaff = await _db.Users.Where(s => !s.Roles.Any(r => r.Name == "Admin" || r.Name == "Customer")).Select(s => new ViewListStaffDTO
            {
                IdStaff = s.Id,
                Email = s.Email,
                Name = s.Username,
                Role = s.Roles.Select(t => t.Name).ToList(),
            }).ToListAsync();

            return listStaff;
        }

        public async Task<List<RoleDTO>> GetListRole()
        {

            var listRole = await _db.Roles.Where(s=>s.Name!="Admin" && s.Name!="Customer").Select(s => new RoleDTO
            {

                IdRole = s.Id,
                RoleName = s.Name,

            }).ToListAsync();


            return listRole;
        }
    }
}
