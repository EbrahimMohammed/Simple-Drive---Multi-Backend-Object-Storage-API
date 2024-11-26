using API.Dtos;
using API.Services;
using Business.UsersRepository;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Account : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly int _expirationDays;

        public Account(IUserRepository userRepository,
                        ITokenService tokenService,
                       IConfiguration config)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _expirationDays = config.GetValue<int>("TokenSettings:ExpirationDays");
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var passwordHasher = new PasswordHasher<string>();
            var hashedPassword = passwordHasher.HashPassword(null, request.Password);

            var user = new User
            {
                UserName = request.Username,
                PasswordHash = hashedPassword
            };

            await _userRepository.Add(user);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.GetByUsername(request.Username);
            if (user == null) return Unauthorized("Invalid username or password.");

            var passwordHasher = new PasswordHasher<string>();
            var verificationResult = passwordHasher.VerifyHashedPassword(null, user.PasswordHash, request.Password);

            if (verificationResult != PasswordVerificationResult.Success)
                return Unauthorized("Invalid username or password.");

            // Generate JWT token
            var token = _tokenService.CreateToken(user);
            return Ok(new { Token = token, Username = user.UserName, ExpireDate = DateTime.Now.AddDays(_expirationDays) });
        }
    }
}
