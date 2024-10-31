using EmploymentSystem.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystem.Application.Services
{
    public interface IUserService
    {
        Task<UserDto> RegisterUserAsync(UserDto userDto);
        Task<string> AuthenticateAsync(LoginDto loginDto);
    }
}
