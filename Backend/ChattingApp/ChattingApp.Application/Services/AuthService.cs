using ChattingApp.Application.DTOs.Auth;
using ChattingApp.Application.Interfaces;
using ChattingApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace ChattingApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Validate user doesn't exist
            var existingUser = await _userRepository.GetUserByUsernameAsync(registerDto.Username);
            if (existingUser != null)
                throw new ArgumentException("Username is already taken.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            var role = registerDto.Username.ToLower() == "admin" ? "Admin" : "User";

            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = passwordHash,
                Role = role
            };

            await _userRepository.CreateUserAsync(user);

            // Fetch created user if needed, or just generate token using the model
            return await GenerateTokenInternalAsync(user);
        }

        public async Task<AuthResponseDto> GenerateTokenAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new ArgumentException("Invalid username or password.");

            return await GenerateTokenInternalAsync(user);
        }

        private Task<AuthResponseDto> GenerateTokenInternalAsync(User user)
        {
            var keyStr = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(keyStr))
                throw new InvalidOperationException("JWT configuration is missing properly.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Task.FromResult(new AuthResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                Username = user.Username
            });
        }
    }
}
