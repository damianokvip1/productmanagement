using System.Security.Cryptography;
using System.Text;
using ProductManagement.DTOs;
using ProductManagement.Models;
using ProductManagement.Repositories;

namespace ProductManagement.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<UserDTO?> CreateUserAsync(UserCreateDTO userCreateDto);
        Task<bool> UpdateUserAsync(int id, UserUpdateDTO userUpdateDto);
        Task<bool> DeleteUserAsync(int id);
    }

    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await userRepository.GetUsersAsync();
            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName
            });
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await userRepository.GetUserByIdAsync(id);
            if (user == null)
                return null;

            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };
        }

        public async Task<UserDTO?> CreateUserAsync(UserCreateDTO userCreateDto)
        {
            var existingUser = await userRepository.GetUserByUserNameAsync(userCreateDto.UserName);
            if (existingUser != null)
                return null;
            var hashedPassword = HashPassword(userCreateDto.Password);

            var user = new User
            {
                UserName = userCreateDto.UserName,
                Email = userCreateDto.Email,
                PasswordHash = hashedPassword
            };

            await userRepository.CreateUserAsync(user);

            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
            };
        }

        public async Task<bool> UpdateUserAsync(int id, UserUpdateDTO userUpdateDto)
        {
            var user = await userRepository.GetUserByIdAsync(id);
            if (user == null)
                return false;

            user.UserName = userUpdateDto.UserName;
            user.Email = userUpdateDto.Email;
            user.PasswordHash = userUpdateDto.Password;

            return await userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id) => await userRepository.DeleteUserAsync(id);

        private static string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashedBytes = SHA256.HashData(bytes);

            var builder = new StringBuilder();
            foreach (var b in hashedBytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
        
    }
}