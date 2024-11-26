using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class RegisterRequest
    {

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
