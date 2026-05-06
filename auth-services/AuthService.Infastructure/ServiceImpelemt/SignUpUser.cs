using auth_services.AuthService.API.gRPCs;
using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Domain.Aggregate;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Domain.ValueObject;
using auth_services.AuthService.Infastructure.DbContextAuth;
using auth_services.AuthService.Infastructure.IntegrationContracts;
using auth_services.AuthService.Infastructure.RabbitMQs.Producer;
using Grpc.Core;
using System.Text.Json;
using UserService.API.Protos;

namespace auth_services.AuthService.Infastructure.ServiceImpelemt
{
    public class SignUpUser : ISignUpUser
    {
        private readonly IGenarateSalt _iGenarateSalt;
        private readonly IHashPassword _iHashPassword;
        private readonly IUserRepository _iUserRepository;
        private readonly RabbitMQProducer _rabbitMQ;
        private readonly IConfiguration _iConfig;
        private readonly UserServicesClient _userClient;
        private readonly FoodAuthContext _db;
        private readonly IOutBoxMessage _outBox;
        private readonly ILogger<SignUpUser> _logger;

        public SignUpUser(ILogger<SignUpUser> logger, IGenarateSalt genarateSalt, IHashPassword hashPassword, IUserRepository userRepository, RabbitMQProducer rabbitMQProducer, IConfiguration configuration, UserServicesClient userServicesClient, FoodAuthContext context, IOutBoxMessage outBoxMessage)
        {
            _iGenarateSalt = genarateSalt;
            _iHashPassword = hashPassword;
            _iUserRepository = userRepository;
            _rabbitMQ = rabbitMQProducer;
            _iConfig = configuration;
            _userClient = userServicesClient;
            _db = context;
            _outBox = outBoxMessage;
            _logger = logger;
        }

        public async Task<bool> Execute(RequestCreateNewUser user, CancellationToken token = default)
        {
            if (user.Password != user.ConfirmPassword) return false;

            if (await _iUserRepository.IsExitUser(user.Email, token)) return false;

            var salt = _iGenarateSalt.GenarateSalt();
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
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Bug At SigUp New User :{ex.Message}");
                await transaction.RollbackAsync();
                return false;
            }

        }
    }
}
