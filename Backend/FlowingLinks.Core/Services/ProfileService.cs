using FlowingLinks.Core.Dtos;
using FlowingLinks.Core.Extensions;
using FlowingLinks.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace FlowingLinks.Core.Services
{
    public class ProfileService
    {
        public ProfileService(FlowingLinksDbContext dbContext) =>
            _dbContext = dbContext;

        private readonly FlowingLinksDbContext _dbContext;

        public async Task Save(UserDto userDto)
        {
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

        public async Task ChangePassword(int id, string password)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                throw new FlowingLinksException($"User with ID {id} not found.");

            user.Password = HashUtils.GenerateMd5Hash(password);

            await _dbContext.SaveChangesAsync();
        }
    }
}