using EmploymentSystem.Domain.Interfaces;
using EmploymentSystem.Domain.Models;
using EmploymentSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        protected readonly EmploymentDbContext _dbContext;

        public UserRepository(EmploymentDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<User> CreateUserAsync(User user)
        {
            await _dbContext.Set<User>().AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            User currentUser = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == userId);

            return currentUser;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            User currentUser = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Username == username);

            return currentUser;

        }
    }
}
