using auth_services.AuthService.API.gRPCs;
using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Domain.Aggregate;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Domain.ValueObject;
using auth_services.AuthService.Infrastructure.DbContextAuth;
using auth_services.AuthService.Infrastructure.IntegrationContracts;
using auth_services.AuthService.Infrastructure.RabbitMQs.Producer;
using Grpc.Core;
using System.Text.Json;
using UserService.API.Protos;

namespace auth_services.AuthService.Infrastructure.ServiceImplement
{
    public class SignUpUser : ISignUpUser
    {
        private readonly IGenerateSalt _iGenerateSalt;
        private readonly IHashPassword _iHashPassword;
        private readonly IUserRepository _iUserRepository;
        private readonly UserServicesClient _userClient;
        private readonly FoodAuthContext _db;
        private readonly IOutBoxMessage _outBox;
        private readonly ILogger<SignUpUser> _logger;

        public SignUpUser(ILogger<SignUpUser> logger, IGenerateSalt generateSalt, IHashPassword hashPassword, IUserRepository userRepository, UserServicesClient userServicesClient, FoodAuthContext context, IOutBoxMessage outBoxMessage)
        {
            _iGenerateSalt = generateSalt;
            _iHashPassword = hashPassword;
            _iUserRepository = userRepository;
            _userClient = userServicesClient;
            _db = context;
            _outBox = outBoxMessage;
            _logger = logger;
        }

        public async Task<ResponseRegisterAccountDto> Execute(RequestCreateNewUser user, CancellationToken token = default)
        {
            if (user.Password != user.ConfirmPassword) return new ResponseRegisterAccountDto { Status = false, Message = "The confirm password not match" };

            if(user.Password.Length < 8) return new ResponseRegisterAccountDto { Status = false, Message = "The password must be at least 8 characters long" };

            if (await _iUserRepository.IsExitUser(user.Email, token)) return new ResponseRegisterAccountDto { Status = false, Message = "The account already exists" };

            var salt = _iGenerateSalt.GenerateSalt();
            var hashedPassword = _iHashPassword.HandleHashPassword(user.Password, salt);

            var userAggregate = UserAggregate.CreateNewUser(new FullNameOfUser(user.UserName), new Email(user.Email), hashedPassword, salt);


            for (int i = 0; i < 3; i++)
            {

                try
                {
                    var resultCallUserClient = await _userClient.CreateNewInformationUserAsync(new CreateNewInformationUserRequest
                    {
                        Id = userAggregate.Id.ToString(),
                        Name = userAggregate.Username.Value,
                        Email = userAggregate.Email.EmailAdress,
                        Phone = "0000000000"
                    });

                    if (resultCallUserClient.IsSuccess)
                    {
                        break;
                    }
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable || ex.StatusCode == StatusCode.DeadlineExceeded)
                {

                    if (i == 2)
                    {
                        throw;
                    }
                    await Task.Delay(200);
                }

            }
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {

                await _iUserRepository.AddNewUser(userAggregate, token);

                var payload = JsonSerializer.Serialize(new RegisterNotificationMessage
                {
                    Email = userAggregate.Email.EmailAdress,
                    Name = userAggregate.Email.EmailAdress,
                    TypeService = "Email"
                });

                await _outBox.CreateNewMessage(new OutBoxMessageInternalDTO("Notification", payload));
                await _db.SaveChangesAsync();


                await transaction.CommitAsync();
                return new ResponseRegisterAccountDto
                {
                    Status = true,
                    Message = "The account has been created successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Bug At SigUp New User :{ex.Message}");
                await transaction.RollbackAsync();
                return new ResponseRegisterAccountDto
                {
                    Status = false,
                    Message = "An error occurred while creating the account"
                };
            }

        }
    }
}
