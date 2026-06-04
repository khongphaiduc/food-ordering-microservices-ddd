using Microsoft.EntityFrameworkCore;
using user_service.userservice.infastructure.DBcontextService;
using user_service.UserService.API.Middlewares;
using user_service.UserService.Application.DTOS;
using user_service.UserService.Application.Services;

namespace user_service.UserService.Infrastructure.ServiceImplement
{
    public class GetInformationUser : IGetInformationUser
    {
        private readonly FoodUsersContext _db;

        public GetInformationUser(FoodUsersContext foodUsersContext)
        {
            _db = foodUsersContext;
        }

        public async Task<InformationUserDTO> Execute(Guid IdUser)
        {
            var user = await _db.Users.Include(s => s.UserAddresses).FirstOrDefaultAsync(s => s.Id == IdUser);

            if (user == null) throw new NotFoundUserException($"Not found user id {IdUser}");


            var inforUser = new InformationUserDTO
            {
                Iduser = user.Id,
                Email = user.Email ?? "Not Found Email",
                Name = user.FullName ?? "Not Found Name User",
                Phone = user.PhoneNumber ?? "Not found phone number their user",

                addressUsers = user.UserAddresses.Select(s => new AddressUser
                {
                    IdAddressItem = s.Id,
                    City = s.City,
                    Line1 = s.AddressLine1,
                    Line2 = s.AddressLine2,
                    Region = s.District
                }).ToList()

            };

            return inforUser;
        }
    }
}
