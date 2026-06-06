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

            repoMock
                .Setup(r => r.ReadByUsername(postModel.Username))
                .ReturnsAsync((User?)null);

            repoMock
                .Setup(r => r.ReadByEmail(postModel.EmailAddress))
                .ReturnsAsync((User?)null);

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

            repoMock
                .Setup(r => r.ReadByUsername(postModel.Username))
                .ReturnsAsync(new User());

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

            repoMock
                .Setup(r => r.ReadByEmail(postModel.EmailAddress))
                .ReturnsAsync(new User());

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
    }
}
