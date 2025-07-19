using FlowingLinks.Core.Models;
using FlowingLinks.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace FlowingLinks.Core.Services
{
    public class LoginService
    {
        public LoginService(FlowingLinksDbContext dbContext) =>
            _dbContext = dbContext;

        private readonly FlowingLinksDbContext _dbContext;

        public async Task<User> Execute(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new FlowingLinksException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(password))
                throw new FlowingLinksException("Password cannot be empty");

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            var invalidMessage = "Invalid username or password";
            if (user == null)
                throw new FlowingLinksException(invalidMessage);

            var hashedPassword = HashUtils.GenerateMd5Hash(password);
            if (user.Password != hashedPassword)
                throw new FlowingLinksException(invalidMessage);

            return user;
        }
    }
}