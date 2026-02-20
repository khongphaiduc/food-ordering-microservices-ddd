using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Domain.Aggregate;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Domain.ValueObject;

namespace auth_services.AuthService.Infastructure.ServiceImpelemt
{
    public class AddAccountStaffs : IAddAccountStaffs
    {
        private readonly IEnumerable<IGanarateTokenJWT> _ganarateToken;
        private readonly IGenarateSalt _Salt;
        private readonly IHashPassword _hashPassword;
        private readonly IUserRepository _userRepo;
        private readonly IAddSafttRepository _addStaff;

        public AddAccountStaffs(IGenarateSalt genarateSalt, IHashPassword hashPassword, IUserRepository userRepository, IAddSafttRepository addSafttRepository)
        {
            _Salt = genarateSalt;
            _hashPassword = hashPassword;
            _userRepo = userRepository;
            _addStaff = addSafttRepository;
        }

        public async Task<bool> AddAccountStaffsAsync(AddAccountStaffDTO accountStaffs)
        {
            if (await _userRepo.IsExitUser(accountStaffs.Email)) return false;

            var salt = _Salt.GenarateSalt();

            var passwordAfterHash = _hashPassword.HandleHashPassword(accountStaffs.Password, salt);

            var NewStart = UserAggregate.CreateNewUser(new FullNameOfUser(accountStaffs.Name), new Domain.ValueObject.Email(accountStaffs.Email), passwordAfterHash, salt);

            var result = await _addStaff.AddSafttAsync(NewStart, accountStaffs.IdRole);

            return result;
        }
    }
}
