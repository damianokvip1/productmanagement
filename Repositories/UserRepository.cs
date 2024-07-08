using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Models;

namespace ProductManagement.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user, string? newPassword = "");
        Task<bool> DeleteUserAsync(int id);
        Task<User?> GetUserByUserNameAsync(string userName);
        Task<bool> ValidatePasswordAsync(User user, string password);
    }

    public class UserRepository(ApplicationDbContext context, IPasswordHasher<User> passwordHasher) : IUserRepository
    {
        public async Task<IEnumerable<User>> GetUsersAsync() => await context.Users.ToListAsync();

        public async Task<User?> GetUserByIdAsync(int id) => await context.Users.FindAsync(id);

        public async Task<User> CreateUserAsync(User user)
        {
            var data = new User
            {
                UserName = user.UserName,
                Email = user.Email,
                PasswordHash = passwordHasher.HashPassword(null, user.PasswordHash)
            };
            context.Users.Add(data);
            await context.SaveChangesAsync();
            return user;
        }
        
        public async Task<bool> UpdateUserAsync(User user, string? newPassword = "")
        {
            if (!String.IsNullOrEmpty(newPassword)) user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
            context.Entry(user).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                    return false;

                throw;
            }
        }
        
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
                return false;
        
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }
        
        public async Task<User?> GetUserByUserNameAsync(string userName) => await context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

        public Task<bool> ValidatePasswordAsync(User user, string password)
        {
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return Task.FromResult(verificationResult == PasswordVerificationResult.Success);
        }

        private bool UserExists(int id) => context.Users.Any(e => e.Id == id);
    }
}