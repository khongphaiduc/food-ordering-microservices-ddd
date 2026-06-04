using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Domain.Aggregate;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Domain.ValueObject;

namespace auth_services.AuthService.Infrastructure.ServiceImplement
{
    public class AddAccountStaffs : IAddAccountStaffs
    {
        private readonly IEnumerable<IGenerateTokenJWT> _generateToken;
        private readonly IGenerateSalt _Salt;
        private readonly IHashPassword _hashPassword;
        private readonly IUserRepository _userRepo;
        private readonly IAddStaffRepository _addStaff;

        public AddAccountStaffs(IGenerateSalt generateSalt, IHashPassword hashPassword, IUserRepository userRepository, IAddStaffRepository addStaffRepository)
        {
            _Salt = generateSalt;
            _hashPassword = hashPassword;
            _userRepo = userRepository;
            _addStaff = addStaffRepository;
        }

        public async Task<bool> AddAccountStaffsAsync(AddAccountStaffDTO accountStaffs, CancellationToken token = default)
        {
            if (await _userRepo.IsExitUser(accountStaffs.Email, token)) return false;

            var salt = _Salt.GenerateSalt();

            var passwordAfterHash = _hashPassword.HandleHashPassword(accountStaffs.Password, salt);

            var NewStart = UserAggregate.CreateNewUser(new FullNameOfUser(accountStaffs.Name), new Domain.ValueObject.Email(accountStaffs.Email), passwordAfterHash, salt);

            var result = await _addStaff.AddStaffAsync(NewStart, accountStaffs.IdRole);

            return result;
        }
    }
}
