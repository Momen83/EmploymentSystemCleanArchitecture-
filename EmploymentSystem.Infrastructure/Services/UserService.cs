using EmploymentSystem.Application.Dtos;
using EmploymentSystem.Application.Services;
using EmploymentSystem.Domain.Interfaces;
using EmploymentSystem.Domain.Models;
using EmploymentSystem.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystem.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly JwtOptions _jwtOptions;
        public UserService(IUserRepository userRepository, JwtOptions jwtOptions , ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
            _jwtOptions = jwtOptions;
            _logger = logger;
        }

        public async Task<string> AuthenticateAsync(LoginDto loginDto)
        {
            if (loginDto.Username == null)
            {
                _logger.LogError($"username is null");
                throw new Exception("username is null");
            }



            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                _logger.LogError($"Invalid username or password.");
                throw new Exception("Invalid username or password.");
            }


            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogError($"Invalid username or password.");
                throw new Exception("Invalid username or password.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SignKey)),
                SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()),
                    new Claim(ClaimTypes.Name,loginDto.Username), // Added null check
                    new Claim(ClaimTypes.Role,user.Role),

                })


            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            _logger.LogInformation($"account with username {loginDto.Username} login successfully");
            return accessToken;

        }

        public async Task<UserDto> RegisterUserAsync(UserDto userDto)
        {
            var existingUser = await _userRepository.GetUserByUsernameAsync(userDto.Username);
            if (existingUser != null)
            {
                _logger.LogError("Username already exists.");
                throw new Exception("Username already exists.");
            }

            if (userDto.Role != "Applicant" && userDto.Role != "Employer")
            {
                _logger.LogError("Invalid Rule");
                throw new Exception("Invalid Role");
            }


            var user = new User
            {
                Username = userDto.Username,
                Role = userDto.Role
            };

            user.Password = _passwordHasher.HashPassword(user, userDto.Password);

            await _userRepository.CreateUserAsync(user);

            userDto.Id = user.Id;
            userDto.Password = null;

            _logger.LogInformation($"account with username {userDto.Username} register successfully");
            return userDto;

        }
    }
}
