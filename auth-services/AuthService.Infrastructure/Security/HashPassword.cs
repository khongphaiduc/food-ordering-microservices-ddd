using auth_services.AuthService.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace auth_services.AuthService.Infrastructure.Security
{
    public class HashPassword : IHashPassword
    {
        public string HandleHashPassword(string password, string salt)
        {
            byte[] PasswordByte = Encoding.UTF8.GetBytes(password);
            byte[] SaltByte = Encoding.UTF8.GetBytes(salt);


            using (var pbkdf2 = new Rfc2898DeriveBytes(PasswordByte, SaltByte, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
