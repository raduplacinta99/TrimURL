using Microsoft.AspNetCore.Identity;
using TrimUrlApi.Entities;
using TrimUrlApi.Enums;
using TrimUrlApi.Models;
using TrimUrlApi.Repositories;

namespace TrimUrlApi.Services
{
    public class UserService(IUserRepository userRepository)
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<UserResponseModel> Create(UserPostModel postModel)
        {
            var user = new User
            {
                Username = postModel.Username,
                PasswordHash = GenerateHash(postModel.Password),
                Role = UserRole.Default,
                EmailAddress = postModel.EmailAddress,
                FullName = postModel.FullName,
            };
            await _userRepository.Create(user);
            return new UserResponseModel(user);
        }

        public async Task<UserResponseModel?> GetByUsername(string username)
        {
            var user = await _userRepository.ReadByUsername(username);
            return (user != null) ? new UserResponseModel(user) : null;

        }

        public async Task<UserResponseModel?> UpdateByUsername(string username, UserPutModel putModel)
        {
            var user = await _userRepository.ReadByUsername(username);
            if (user == null)
            {
                return null;
            }

            if (putModel.Password != null)
            {
                user.PasswordHash = GenerateHash(putModel.Password);
            }
            if (putModel.EmailAddress != null)
            {
                user.EmailAddress = putModel.EmailAddress;
            }
            await _userRepository.Update(user);
            return new UserResponseModel(user);
        }

        public async Task<User?> DeleteByUsername(string username)
        {
            var user = await _userRepository.ReadByUsername(username);
            if (user == null)
            {
                return null;
            }

            await _userRepository.DeleteById(user.Id);
            return user;
        }

        public async Task<bool> IsUsernameAvailable(string username)
        {
            return await _userRepository.ReadByUsername(username) == null;
        }

        public async Task<bool> IsEmailAvailable(string email)
        {
            return await _userRepository.ReadByEmail(email) == null;
        }

        private static string GenerateHash(string password)
        {
            var hasher = new PasswordHasher<string>();
            return hasher.HashPassword("", password);
        }
    }
}
