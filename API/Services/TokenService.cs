using Data.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    public class TokenService: ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly int _expirationDays;

        public TokenService(IConfiguration config)
        {
            // Retrieve TokenKey and ExpirationDays from appsettings.json
            var tokenKey = config["TokenSettings:TokenKey"];
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            _expirationDays = config.GetValue<int>("TokenSettings:ExpirationDays");
        }

        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName),
                new Claim("UserId", user.Id.ToString())
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(_expirationDays), // Use configured ExpirationDays
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
