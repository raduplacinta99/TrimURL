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
        [Fact]
        public async Task Create_ShouldCreateUser_WhenUsernameAndEmailAreAvailable()
        {
            var repoMock = new Mock<IUserRepository>();
            var service = new UserService(repoMock.Object);

            // Arrange
            var postModel = new UserPostModel
            {
                Username = "john",
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

            // Act
            var result = await service.Create(postModel);

            // Assert
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
                Username = "john",
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
                Username = "john",
                Password = "password123",
                EmailAddress = "john@test.com"
            };

            repoMock
                .Setup(r => r.ReadByEmail(postModel.EmailAddress))
                .ReturnsAsync(new User());

            await Assert.ThrowsAsync<UnavailableEmailException>(
                () => service.Create(postModel));
        }
    }
}
