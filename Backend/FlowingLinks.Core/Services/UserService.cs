using FlowingLinks.Core.Dtos;
using FlowingLinks.Core.Extensions;
using FlowingLinks.Core.Models;
using FlowingLinks.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace FlowingLinks.Core.Services
{
    public class UserService
    {
        public UserService(FlowingLinksDbContext dbContext) =>
            _dbContext = dbContext;

        private readonly FlowingLinksDbContext _dbContext;

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User?> GetById(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task Save(UserDto userDto)
        {
            if (userDto.Id == 0)
            {
                // Creating new user
                // Check if username already exists
                var existingUser = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.Username == userDto.Username);
                
                if (existingUser != null)
                    throw new FlowingLinksException($"Username '{userDto.Username}' already exists.");

                var user = userDto.CopyTo<User>();
                user.Password = HashUtils.GenerateMd5Hash("123456");
                _dbContext.Users.Add(user);
                
                await _dbContext.SaveChangesAsync();
                userDto.Id = user.Id;
            }
            else
            {
                // Updating existing user
                var user = await _dbContext.Users.FindAsync(userDto.Id);
                if (user == null)
                    throw new FlowingLinksException($"User with ID {userDto.Id} not found.");

                // Check if username already exists (excluding current user)
                var duplicateUsernameUser = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.Username == userDto.Username && x.Id != userDto.Id);
                
                if (duplicateUsernameUser != null)
                    throw new FlowingLinksException($"Username '{userDto.Username}' already exists.");

                userDto.CopyTo(user);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> Delete(int id)
        {
            if (id == 1)
                throw new FlowingLinksException("The admin user can't be deleted.");

            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return false;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UsernameExists(string username)
        {
            var result = await _dbContext.Users.AnyAsync(x => x.Username == username);
            return result;
        }
    }
}