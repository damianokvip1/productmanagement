using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ProductManagement.DTOs;
using ProductManagement.Models;
using ProductManagement.Repositories;

namespace ProductManagement.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto.UserData>> GetAllUsersAsync();
        Task<UserDto.UserData?> GetUserByIdAsync(int id);
        Task<UserDto.UserData?> CreateUserAsync(UserDto.UserCreate userCreateDto);
        Task<bool> UpdateUserAsync(int id, UserDto.UserUpdate userUpdateDto);
        Task<bool> DeleteUserAsync(int id);
        Task<User?> ValidateCredentials(AuthDto.Login request);
        Task<bool> ChangePasswordAsync(string id, string currentPassword, string newPassword);
    }

    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<IEnumerable<UserDto.UserData>> GetAllUsersAsync()
        {
            var users = await userRepository.GetUsersAsync();
            return users.Select(u => new UserDto.UserData
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName
            });
        }

        public async Task<UserDto.UserData?> GetUserByIdAsync(int id)
        {
            var user = await userRepository.GetUserByIdAsync(id);
            if (user == null)
                return null;

            return new UserDto.UserData
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };
        }

        public async Task<UserDto.UserData?> CreateUserAsync(UserDto.UserCreate userCreateDto)
        {
            var existingUser = await userRepository.GetUserByUserNameAsync(userCreateDto.UserName);
            if (existingUser != null)
                return null;

            var user = new User
            {
                UserName = userCreateDto.UserName,
                Email = userCreateDto.Email,
                PasswordHash = userCreateDto.Password
            };

            await userRepository.CreateUserAsync(user);

            return new UserDto.UserData
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
            };
        }

        public async Task<bool> UpdateUserAsync(int id, UserDto.UserUpdate userUpdateDto)
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
        
        private async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await userRepository.ValidatePasswordAsync(user, password);
        }
        
        private async Task<User?> FindByUsernameAsync(string username)
        {
            return await userRepository.GetUserByUserNameAsync(username);
        }

        public async Task<User?> ValidateCredentials(AuthDto.Login request)
        {
            var user = await FindByUsernameAsync(request.UserName);
            if (user == null || !await CheckPasswordAsync(user, request.Password))
                return null;

            return user;
        }
        
        public async Task<bool> ChangePasswordAsync(string id, string currentPassword, string newPassword)
        {
            var user = await userRepository.GetUserByIdAsync(int.Parse(id));
            if (user == null) return false;

            var validate = await CheckPasswordAsync(user, currentPassword);;
            if (!validate) return false;

            await userRepository.UpdateUserAsync(user, newPassword);
            return true;
        }

    }
}