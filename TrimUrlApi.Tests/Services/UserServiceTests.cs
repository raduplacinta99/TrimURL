using Microsoft.AspNetCore.Identity;
using Moq;
using TrimUrlApi.Entities;
using TrimUrlApi.Exceptions;
using TrimUrlApi.Models;
using TrimUrlApi.Repositories;
using TrimUrlApi.Services;

namespace TrimUrlApi.Tests.Services
{
    public class UserServiceTests
    {
        private const string ValidUsername = "john";
        private const string InvalidUsername = "does-not-exist";

        private readonly Mock<IUserRepository> _repoMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _repoMock = new Mock<IUserRepository>();
            _service = new UserService(_repoMock.Object);
        }

        [Fact]
        public async Task Create_ShouldCreateUser_WhenUsernameAndEmailAreAvailable()
        {
            var postModel = new UserPostModel
            {
                Username = ValidUsername,
                Password = "password123",
                EmailAddress = "john@test.com",
                FullName = "John Doe"
            };

            _repoMock.Setup(r => r.ReadByUsername(postModel.Username)).ReturnsAsync((User?)null);
            _repoMock.Setup(r => r.ReadByEmail(postModel.EmailAddress)).ReturnsAsync((User?)null);

            User? createdUser = null;

            _repoMock
                .Setup(r => r.Create(It.IsAny<User>()))
                .Callback<User>(u => createdUser = u)
                .Returns(Task.CompletedTask);

            var result = await _service.Create(postModel);

            Assert.NotNull(result);
            Assert.Equal(postModel.Username, result.Username);
            Assert.Equal(postModel.EmailAddress, result.EmailAddress);
            Assert.NotNull(createdUser);
            Assert.NotEqual(postModel.Password, createdUser!.PasswordHash);
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenUsernameAlreadyExists()
        {
            var postModel = new UserPostModel
            {
                Username = ValidUsername,
                Password = "password123",
                EmailAddress = "john@test.com"
            };

            _repoMock.Setup(r => r.ReadByUsername(postModel.Username)).ReturnsAsync(new User());

            await Assert.ThrowsAsync<UnavailableUsernameException>(
                () => _service.Create(postModel));
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenEmailAlreadyExists()
        {
            var postModel = new UserPostModel
            {
                Username = ValidUsername,
                Password = "password123",
                EmailAddress = "john@test.com"
            };

            _repoMock.Setup(r => r.ReadByEmail(postModel.EmailAddress)).ReturnsAsync(new User());

            await Assert.ThrowsAsync<UnavailableEmailException>(
                () => _service.Create(postModel));
        }

        [Fact]
        public async Task GetByUsername_ShouldReturnUserResponseModel_WhenUserExists()
        {
            var user = new User
            {
                Username = ValidUsername,
                EmailAddress = "john@test.com",
                FullName = "John Doe"
            };

            _repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);

            var result = await _service.GetByUsername(ValidUsername);

            Assert.NotNull(result);
            Assert.Equal(ValidUsername, result.Username);
        }

        [Fact]
        public async Task GetByUsername_ShouldThrowException_WhenUserDoesNotExist()
        {
            _repoMock.Setup(r => r.ReadByUsername(InvalidUsername)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<UsernameNotFoundException>(() =>
                _service.GetByUsername(InvalidUsername));
        }

        [Fact]
        public async Task UpdateByUsername_ShouldUpdateEmail_WhenEmailIsAvailable()
        {
            var newEmail = "new@test.com";
            var user = new User
            {
                Id = 1,
                Username = ValidUsername,
                PasswordHash = "oldhash",
                EmailAddress = "old@test.com"
            };

            var putModel = new UserPutModel
            {
                EmailAddress = newEmail
            };

            _repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);
            _repoMock.Setup(r => r.ReadByEmail(newEmail)).ReturnsAsync((User?)null);

            var result = await _service.UpdateByUsername(ValidUsername, putModel);

            Assert.Equal(newEmail, user.EmailAddress);
        }

        [Fact]
        public async Task UpdateByUsername_ShouldUpdatePassword()
        {
            var newPassword = "newPassword123";
            var user = new User
            {
                Id = 1,
                Username = ValidUsername,
                PasswordHash = "oldhash",
                EmailAddress = "john@test.com"
            };

            var putModel = new UserPutModel
            {
                Password = newPassword
            };

            _repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);

            await _service.UpdateByUsername(ValidUsername, putModel);

            Assert.NotEqual("oldhash", user.PasswordHash);

            var hasher = new PasswordHasher<string>();

            var result = hasher.VerifyHashedPassword("",user.PasswordHash,newPassword);

            Assert.Equal(PasswordVerificationResult.Success,result);
        }

        [Fact]
        public async Task UpdateByUsername_ShouldThrowException_WhenUserDoesNotExist()
        {
            _repoMock.Setup(r => r.ReadByUsername(InvalidUsername)).ReturnsAsync((User?)null);

            var newEmail = "new@test.com";
            var putModel = new UserPutModel
            {
                EmailAddress = newEmail
            };

            await Assert.ThrowsAsync<UsernameNotFoundException>(() =>
                _service.UpdateByUsername(InvalidUsername, putModel));
        }

        [Fact]
        public async Task UpdateByUsername_ShouldThrowException_WhenEmailAlreadyExists()
        {
            var user = new User
            {
                Id = 1,
                Username = ValidUsername,
                PasswordHash = "oldhash",
                EmailAddress = "old@test.com"
            };

            var putModel = new UserPutModel
            {
                EmailAddress = "john@test.com"
            };

            _repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);
            _repoMock.Setup(r => r.ReadByEmail(putModel.EmailAddress)).ReturnsAsync(new User());

            await Assert.ThrowsAsync<UnavailableEmailException>(() => _service.UpdateByUsername(ValidUsername, putModel));
        }

        [Fact]
        public async Task UpdateByUsername_ShouldThrowException_WhenMissingUpdateFields()
        {
            var user = new User
            {
                Id = 1,
                Username = ValidUsername,
                PasswordHash = "oldhash",
                EmailAddress = "old@test.com"
            };

            var putModel = new UserPutModel();

            _repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);

            await Assert.ThrowsAsync<MissingUserUpdateFieldsException>(() => _service.UpdateByUsername(ValidUsername, putModel));
        }

        [Fact]
        public async Task DeleteByUsername_ShouldReturnUser_WhenUserExists()
        {
            var newEmail = "new@test.com";

            var user = new User
            {
                Username = ValidUsername,
                EmailAddress = "john@test.com",
                FullName = "John Doe"
            };

            _repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);

            var result = await _service.DeleteByUsername(ValidUsername);

            Assert.NotNull(result);
            Assert.Equal(ValidUsername, result.Username);
        }

        [Fact]
        public async Task DeleteByUsername_ShoulThrowException_WhenUserDoesNotExist()
        {
            _repoMock.Setup(r => r.ReadByUsername(InvalidUsername)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<UsernameNotFoundException>(() =>
                _service.DeleteByUsername(InvalidUsername));
        }
    }
}
