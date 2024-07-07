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
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<User?> GetUserByUserNameAsync(string userName);
    }

    public class UserRepository(ApplicationDbContext context) : IUserRepository
    {
        public async Task<IEnumerable<User>> GetUsersAsync() => await context.Users.ToListAsync();

        public async Task<User?> GetUserByIdAsync(int id) => await context.Users.FindAsync(id);

        public async Task<User> CreateUserAsync(User user)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }
        
        public async Task<bool> UpdateUserAsync(User user)
        {
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

        private bool UserExists(int id) => context.Users.Any(e => e.Id == id);
    }
}