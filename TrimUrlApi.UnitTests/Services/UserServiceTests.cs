using Microsoft.AspNetCore.Identity;
using Moq;
using TrimUrlApi.Entities;
using TrimUrlApi.Exceptions;
using TrimUrlApi.Models;
using TrimUrlApi.Repositories;
using TrimUrlApi.Services;

namespace TrimUrlApi.UnitTests.Services
{
    public class UserServiceTests
    {
        private const string ValidUsername = "john";
        private const string StrongPassword = "DG3qTJdgz@5ZPUegfQcb";

        private const string InvalidUsername = "does-not-exist";

        private readonly Mock<IUserRepository> _repoMock;
        private readonly UserService _service;
        private readonly Mock<IPasswordHasher<string>> _hasher;

        public UserServiceTests()
        {
            _repoMock = new Mock<IUserRepository>();
            _hasher = new Mock<IPasswordHasher<string>>();
            _service = new UserService(_repoMock.Object, _hasher.Object);
        }

        [Fact]
        public async Task Create_ShouldCreateUser_WhenUsernameAndEmailAreAvailable()
        {
            var postModel = new UserPostModel
            {
                Username = ValidUsername,
                Password = StrongPassword,
                EmailAddress = "john@test.com",
                FullName = "John Doe"
            };

            _repoMock.Setup(r => r.ReadByUsername(postModel.Username)).ReturnsAsync((User?)null);
            _repoMock.Setup(r => r.ReadByEmail(postModel.EmailAddress)).ReturnsAsync((User?)null);
            _hasher.Setup(h => h.HashPassword("", postModel.Password)).Returns("hashed-password");

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
            Assert.Equal("hashed-password", createdUser!.PasswordHash);
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenUsernameAlreadyExists()
        {
            var postModel = new UserPostModel
            {
                Username = ValidUsername,
                Password = StrongPassword,
                EmailAddress = "john@test.com"
            };

            _repoMock.Setup(r => r.ReadByUsername(postModel.Username)).ReturnsAsync(new User());

            await Assert.ThrowsAsync<UnavailableUsernameException>(
                () => _service.Create(postModel));
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenUsernameIsInvalid()
        {
            var postModel = new UserPostModel
            {
                Username = "john smith",
                Password = StrongPassword,
                EmailAddress = "john@test.com"
            };

            _repoMock.Setup(r => r.ReadByUsername(postModel.Username)).ReturnsAsync((User?)null);
            _repoMock.Setup(r => r.ReadByEmail(postModel.EmailAddress)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<InvalidFieldException>(
                () => _service.Create(postModel));
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenPasswordIsWeak()
        {
            var postModel = new UserPostModel
            {
                Username = ValidUsername,
                Password = "weak-password",
                EmailAddress = "john@test.com"
            };

            _repoMock.Setup(r => r.ReadByUsername(postModel.Username)).ReturnsAsync((User?)null);
            _repoMock.Setup(r => r.ReadByEmail(postModel.EmailAddress)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<InvalidFieldException>(
                () => _service.Create(postModel));
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenFullNameIsInvalid()
        {
            var postModel = new UserPostModel
            {
                Username = ValidUsername,
                Password = StrongPassword,
                EmailAddress = "john@test.com",
                FullName = "J0hn Sm!th"
            };

            _repoMock.Setup(r => r.ReadByUsername(postModel.Username)).ReturnsAsync((User?)null);
            _repoMock.Setup(r => r.ReadByEmail(postModel.EmailAddress)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<InvalidFieldException>(
                () => _service.Create(postModel));
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenEmailAlreadyExists()
        {
            var postModel = new UserPostModel
            {
                Username = ValidUsername,
                Password = StrongPassword,
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

            var correctPassword = "!Correct-p@ssword";
            var putModel = new UserPutModel
            {
                CurrentPassword = correctPassword,
                EmailAddress = newEmail
            };

            _repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);
            _repoMock.Setup(r => r.ReadByEmail(newEmail)).ReturnsAsync((User?)null);
            _hasher.Setup(h => h.VerifyHashedPassword("", user.PasswordHash, correctPassword)).Returns(PasswordVerificationResult.Success);

            var result = await _service.UpdateByUsername(ValidUsername, putModel);

            Assert.Equal(newEmail, user.EmailAddress);
        }

        [Fact]
        public async Task UpdateByUsername_ShouldUpdatePassword()
        {
            var newPassword = "PBawf!7ZXABqamkAzkXvR";
            var user = new User
            {
                Id = 1,
                Username = ValidUsername,
                PasswordHash = "oldhash",
                EmailAddress = "john@test.com"
            };

            var correctPassword = "!Correct-p@ssword";
            var putModel = new UserPutModel
            {
                CurrentPassword = correctPassword,
                NewPassword = newPassword
            };

            _repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);
            _hasher.Setup(h => h.VerifyHashedPassword("", user.PasswordHash, correctPassword)).Returns(PasswordVerificationResult.Success);
            _hasher.Setup(h => h.HashPassword("", newPassword)).Returns("newhash");

            await _service.UpdateByUsername(ValidUsername, putModel);

            Assert.NotEqual("oldhash", user.PasswordHash);
            _hasher.Verify(h => h.HashPassword("", newPassword), Times.Once);
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

            var correctPassword = "!Correct-p@ssword";
            var putModel = new UserPutModel
            {
                CurrentPassword = correctPassword,
                EmailAddress = "john@test.com"
            };

            _repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);
            _repoMock.Setup(r => r.ReadByEmail(putModel.EmailAddress)).ReturnsAsync(new User());
            _hasher.Setup(h => h.VerifyHashedPassword("", user.PasswordHash, correctPassword)).Returns(PasswordVerificationResult.Success);

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
        public async Task UpdateByUsername_ShouldThrowException_WhenPasswordIsWeak()
        {
            var user = new User
            {
                Id = 1,
                Username = ValidUsername,
                PasswordHash = "oldhash",
                EmailAddress = "old@test.com"
            };

            var correctPassword = "!Correct-p@ssword";
            var putModel = new UserPutModel
            {
                CurrentPassword = correctPassword,
                NewPassword = "weak-password"
            };

            _repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);
            _hasher.Setup(h => h.VerifyHashedPassword("", user.PasswordHash, correctPassword)).Returns(PasswordVerificationResult.Success);

            await Assert.ThrowsAsync<InvalidFieldException>(() => _service.UpdateByUsername(ValidUsername, putModel));
        }

        [Fact]
        public async Task UpdateByUsername_ShouldThrowException_WhenCurrentPasswordIsIncorrect()
        {
            var user = new User
            {
                Id = 1,
                Username = ValidUsername,
                PasswordHash = "oldhash",
                EmailAddress = "old@test.com"
            };

            var wrongPassword = "!Wrong-p@ssword1";

            var putModel = new UserPutModel
            {
                CurrentPassword = wrongPassword,
                NewPassword = "PBawf!7ZXABqamkAzkXvR",
                EmailAddress = "new@test.com"
            };

            _repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);
            _hasher.Setup(h => h.VerifyHashedPassword("", user.PasswordHash, wrongPassword)).Returns(PasswordVerificationResult.Failed);

            await Assert.ThrowsAsync<IncorrectPasswordException>(() => _service.UpdateByUsername(ValidUsername, putModel));
        }

        [Fact]
        public async Task UpdateByUsername_ShouldThrowException_WhenCurrentPasswordIsMissing()
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
                EmailAddress = "new@test.com"
            };

            _repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);

            await Assert.ThrowsAsync<IncorrectPasswordException>(() => _service.UpdateByUsername(ValidUsername, putModel));
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
