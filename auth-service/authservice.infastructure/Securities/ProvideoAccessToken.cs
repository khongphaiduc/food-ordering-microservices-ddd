using auth_service.authservice.api.CustomExceptionSerives;
using auth_service.authservice.application.dtos;
using auth_service.authservice.application.InterfaceApplication;
using auth_service.authservice.infastructure.dbcontexts;
using Microsoft.EntityFrameworkCore;

namespace auth_service.authservice.infastructure.Securities
{
    public class ProvideoAccessToken : IProvideoAccessToken
    {
        private readonly FoodAuthContext _db;
        private readonly IAuthenticationToken _Iauthen;

        public ProvideoAccessToken(FoodAuthContext foodAuth, IAuthenticationToken authenticationToken)
        {
            _db = foodAuth;
            _Iauthen = authenticationToken;
        }

        public async Task<TokenResult> ProvideAccessToken(Guid userId)
        {

            var user = await _db.Users.FirstOrDefaultAsync(s => s.Id == userId);

            if (user == null)
            {
                throw new NotFoundException("User not exits");
            }

            var token = _Iauthen.GenerateToken(user.Email, "Customer", "AccessToken");

            return new TokenResult
            {
                TypeToken = "AccessToken",
                Token = token.Token,
                TimeCreate = token.TimeCreate,
                TimeExpire = token.TimeExpire
            };

        }
    }
}
