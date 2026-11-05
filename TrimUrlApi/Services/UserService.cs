using Microsoft.AspNetCore.Identity;
using TrimUrlApi.Entities;
using TrimUrlApi.Enums;
using TrimUrlApi.Exceptions;
using TrimUrlApi.Models;
using TrimUrlApi.Repositories;
using TrimUrlApi.Validators;

namespace TrimUrlApi.Services
{
    public class UserService(IUserRepository userRepository, IPasswordHasher<string> hasher) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher<string> _hasher = hasher;

        public async Task<UserResponseModel> Create(UserPostModel postModel)
        {
            if (!await IsUsernameAvailable(postModel.Username))
            {
                throw new UnavailableUsernameException(postModel.Username);
            }

            if (!await IsEmailAvailable(postModel.EmailAddress))
            {
                throw new UnavailableEmailException(postModel.EmailAddress);
            }

            UserValidator.ValidateUsername(postModel.Username);
            UserValidator.ValidatePassword(postModel.Password);
            UserValidator.ValidateFullName(postModel.FullName);

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
            var user = await GetByUsernameOrThrow(username);
            return new UserResponseModel(user);
        }

        public async Task<UserResponseModel?> UpdateByUsername(string username, UserPutModel putModel)
        {
            var user = await GetByUsernameOrThrow(username);

            if (putModel.NewPassword == null && putModel.EmailAddress == null)
            {
                throw new MissingUserUpdateFieldsException(username);
            }

            if (putModel.CurrentPassword == null)
            {
                throw new IncorrectPasswordException();
            }

            VerifyPassword(putModel.CurrentPassword, user.PasswordHash);

            if (putModel.NewPassword != null)
            {
                UserValidator.ValidatePassword(putModel.NewPassword);
                user.PasswordHash = GenerateHash(putModel.NewPassword);
            }
            if (putModel.EmailAddress != null)
            {
                if (!await IsEmailAvailable(putModel.EmailAddress))
                {
                    throw new UnavailableEmailException(putModel.EmailAddress);
                }

                user.EmailAddress = putModel.EmailAddress;
            }

            await _userRepository.Update(user);
            return new UserResponseModel(user);
        }

        public async Task<User?> DeleteByUsername(string username)
        {
            var user = await GetByUsernameOrThrow(username);
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

        private async Task<User> GetByUsernameOrThrow(string username)
        {
            var user = await _userRepository.ReadByUsername(username);
            if (user == null)
            {
                throw new UsernameNotFoundException(username);
            }
            return user;
        }

        private void VerifyPassword(string password, string passwordHash)
        {
            var result = _hasher.VerifyHashedPassword("", passwordHash, password);
            if (result != PasswordVerificationResult.Success)
            {
                throw new IncorrectPasswordException();
            }
        }

        private string GenerateHash(string password)
        {
            return _hasher.HashPassword("", password);
        }
    }
}
