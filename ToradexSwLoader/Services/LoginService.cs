using Microsoft.EntityFrameworkCore;
using System.Text;
using ToradexSwLoader.Data;

namespace ToradexSwLoader.Services
{
    public class LoginService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public int UserId { get; private set; }
        public string Username { get; private set; } = string.Empty;

        public event Action? OnLoginChanged;

        public LoginService(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void SetLogin(int userId, string username)
        {
            UserId = userId;
            Username = username;
            OnLoginChanged?.Invoke();
        }

        public async Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            var user = await context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
                return false;

            if (user.Password != currentPassword)
                return false;

            user.Password = newPassword;
            await context.SaveChangesAsync();
            return true;
        }
    }
}
