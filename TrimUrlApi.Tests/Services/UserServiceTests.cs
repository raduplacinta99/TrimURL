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

        [Fact]
        public async Task Create_ShouldCreateUser_WhenUsernameAndEmailAreAvailable()
        {
            var repoMock = new Mock<IUserRepository>();
            var service = new UserService(repoMock.Object);

            var postModel = new UserPostModel
            {
                Username = ValidUsername,
                Password = "password123",
                EmailAddress = "john@test.com",
                FullName = "John Doe"
            };

            repoMock.Setup(r => r.ReadByUsername(postModel.Username)).ReturnsAsync((User?)null);
            repoMock.Setup(r => r.ReadByEmail(postModel.EmailAddress)).ReturnsAsync((User?)null);

            User? createdUser = null;

            repoMock
                .Setup(r => r.Create(It.IsAny<User>()))
                .Callback<User>(u => createdUser = u)
                .Returns(Task.CompletedTask);

            var result = await service.Create(postModel);

            Assert.NotNull(result);
            Assert.Equal(postModel.Username, result.Username);
            Assert.Equal(postModel.EmailAddress, result.EmailAddress);
            Assert.NotNull(createdUser);
            Assert.NotEqual(postModel.Password, createdUser!.PasswordHash);
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenUsernameAlreadyExists()
        {
            var repoMock = new Mock<IUserRepository>();
            var service = new UserService(repoMock.Object);

            var postModel = new UserPostModel
            {
                Username = ValidUsername,
                Password = "password123",
                EmailAddress = "john@test.com"
            };

            repoMock.Setup(r => r.ReadByUsername(postModel.Username)).ReturnsAsync(new User());

            await Assert.ThrowsAsync<UnavailableUsernameException>(
                () => service.Create(postModel));
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenEmailAlreadyExists()
        {
            var repoMock = new Mock<IUserRepository>();
            var service = new UserService(repoMock.Object);

            var postModel = new UserPostModel
            {
                Username = ValidUsername,
                Password = "password123",
                EmailAddress = "john@test.com"
            };

            repoMock.Setup(r => r.ReadByEmail(postModel.EmailAddress)).ReturnsAsync(new User());

            await Assert.ThrowsAsync<UnavailableEmailException>(
                () => service.Create(postModel));
        }

        [Fact]
        public async Task GetByUsername_ShouldReturnUserResponseModel_WhenUserExists()
        {
            var repoMock = new Mock<IUserRepository>();
            var service = new UserService(repoMock.Object);

            var user = new User
            {
                Username = ValidUsername,
                EmailAddress = "john@test.com",
                FullName = "John Doe"
            };

            repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);

            var result = await service.GetByUsername(ValidUsername);

            Assert.NotNull(result);
            Assert.Equal(ValidUsername, result.Username);
        }

        [Fact]
        public async Task GetByUsername_ShouldThrowException_WhenUserDoesNotExist()
        {
            var repoMock = new Mock<IUserRepository>();
            var service = new UserService(repoMock.Object);

            repoMock.Setup(r => r.ReadByUsername(InvalidUsername)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<UsernameNotFoundException>(() =>
                service.GetByUsername(InvalidUsername));
        }

        [Fact]
        public async Task UpdateByUsername_ShouldUpdateEmail_WhenEmailIsAvailable()
        {
            var repoMock = new Mock<IUserRepository>();
            var service = new UserService(repoMock.Object);
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

            repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);
            repoMock.Setup(r => r.ReadByEmail(newEmail)).ReturnsAsync((User?)null);

            var result = await service.UpdateByUsername(ValidUsername, putModel);

            Assert.Equal(newEmail, user.EmailAddress);
        }

        [Fact]
        public async Task UpdateByUsername_ShouldUpdatePassword()
        {
            var repoMock = new Mock<IUserRepository>();
            var service = new UserService(repoMock.Object);
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

            repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);

            await service.UpdateByUsername(ValidUsername, putModel);

            Assert.NotEqual("oldhash", user.PasswordHash);

            var hasher = new PasswordHasher<string>();

            var result = hasher.VerifyHashedPassword("",user.PasswordHash,newPassword);

            Assert.Equal(PasswordVerificationResult.Success,result);
        }

        [Fact]
        public async Task Update_ShouldThrowException_WhenEmailAlreadyExists()
        {
            var repoMock = new Mock<IUserRepository>();
            var service = new UserService(repoMock.Object);
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

            repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);
            repoMock.Setup(r => r.ReadByEmail(putModel.EmailAddress)).ReturnsAsync(new User());

            await Assert.ThrowsAsync<UnavailableEmailException>(() => service.UpdateByUsername(ValidUsername, putModel));
        }

        [Fact]
        public async Task Update_ShouldThrowException_WhenMissingUpdateFields()
        {
            var repoMock = new Mock<IUserRepository>();
            var service = new UserService(repoMock.Object);
            var user = new User
            {
                Id = 1,
                Username = ValidUsername,
                PasswordHash = "oldhash",
                EmailAddress = "old@test.com"
            };

            var putModel = new UserPutModel();

            repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);

            await Assert.ThrowsAsync<MissingUserUpdateFieldsException>(() => service.UpdateByUsername(ValidUsername, putModel));
        }

        [Fact]
        public async Task DeleteByUsername_ShouldReturnUser_WhenUserExists()
        {
            var repoMock = new Mock<IUserRepository>();
            var service = new UserService(repoMock.Object);
            var newEmail = "new@test.com";

            var user = new User
            {
                Username = ValidUsername,
                EmailAddress = "john@test.com",
                FullName = "John Doe"
            };

            repoMock.Setup(r => r.ReadByUsername(ValidUsername)).ReturnsAsync(user);

            var result = await service.DeleteByUsername(ValidUsername);

            Assert.NotNull(result);
            Assert.Equal(ValidUsername, result.Username);
        }

        [Fact]
        public async Task DeleteByUsername_ShoulThrowException_WhenUserDoesNotExist()
        {
            var repoMock = new Mock<IUserRepository>();
            var service = new UserService(repoMock.Object);

            repoMock.Setup(r => r.ReadByUsername(InvalidUsername)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<UsernameNotFoundException>(() =>
                service.DeleteByUsername(InvalidUsername));
        }
    }
}
