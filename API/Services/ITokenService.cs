using Data.Models;

namespace API.Services
{
    public interface ITokenService
    {
        public string CreateToken(User user);

    }
}
