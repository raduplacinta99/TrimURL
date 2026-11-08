using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
using TrimUrlApi.Database;
using TrimUrlApi.IntegrationTests.Helpers;
using TrimUrlApi.IntegrationTests.Infrastructure;
using TrimUrlApi.Models;
using Xunit.Abstractions;

namespace TrimUrlApi.IntegrationTests.Users
{
    public class UpdateTests : IClassFixture<TrimUrlWebApplicationFactory>
    {
        private readonly TrimUrlWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public UpdateTests(TrimUrlWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();
            db.Database.EnsureCreated();
        }

        [Fact]
        public async Task Update_ShouldUpdateEmail()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            await DatabaseSeeder.SeedUserAsync(
                db,
                username: "john",
                email: "john@test.com",
                password: "Password123!",
                fullName: "John Smith"
            );

            var token = await _client.LoginAndGetToken("john", "Password123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.AuthenticatedUser,
                new
                {
                    CurrentPassword = "Password123!",
                    EmailAddress = "johnsmith@test.com"
                });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var userModel = await response.Content.ReadFromJsonAsync<UserResponseModel>();
            Assert.NotNull(userModel);
            Assert.Equal("johnsmith@test.com", userModel.EmailAddress);
        }

        [Fact]
        public async Task Update_ShouldUpdatePassword()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            await DatabaseSeeder.SeedUserAsync(
                db,
                username: "john",
                email: "john@test.com",
                password: "Password123!",
                fullName: "John Smith"
            );

            var token = await _client.LoginAndGetToken("john", "Password123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.AuthenticatedUser,
                new
                {
                    CurrentPassword = "Password123!",
                    NewPassword = "!P@ssw0rd&"
                });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnUnauthorized_WhenCurrentPasswordIsIncorrect()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            await DatabaseSeeder.SeedUserAsync(
                db,
                username: "john",
                email: "john@test.com",
                password: "Password123!",
                fullName: "John Smith"
            );

            var token = await _client.LoginAndGetToken("john", "Password123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.AuthenticatedUser,
                new
                {
                    CurrentPassword = "WrongPassword123!",
                    NewPassword = "!P@ssw0rd&",
                    EmailAddress = "johnsmith@test.com"
                });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnUnauthorized_WhenCurrentPasswordIsMissing()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            await DatabaseSeeder.SeedUserAsync(
                db,
                username: "john",
                email: "john@test.com",
                password: "Password123!",
                fullName: "John Smith"
            );

            var token = await _client.LoginAndGetToken("john", "Password123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.AuthenticatedUser,
                new
                {
                    EmailAddress = "johnsmith@test.com"
                });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnUnauthorized_WhenTokenIsMissng()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.AuthenticatedUser,
                new
                {
                    EmailAddress = "john.smith@test.com",
                });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnConflict_WhenEmailIsUnavailable()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            await DatabaseSeeder.SeedUserAsync(
                db,
                id: 1,
                username: "john",
                email: "john@test.com",
                password: "Password123!",
                fullName: "John Smith"
            );

            await DatabaseSeeder.SeedUserAsync(
                db,
                id: 2,
                username: "johnny",
                email: "johnny@test.com",
                password: "p@ssw0rD&&&",
                fullName: "Johnny Cash"
            );

            var token = await _client.LoginAndGetToken("john", "Password123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.AuthenticatedUser,
                new
                {
                    CurrentPassword = "Password123!",
                    EmailAddress = "johnny@test.com"
                });

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenPasswordIsWeak()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            await DatabaseSeeder.SeedUserAsync(
                db,
                username: "john",
                email: "john@test.com",
                password: "Password123!",
                fullName: "John Smith"
            );

            var token = await _client.LoginAndGetToken("john", "Password123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.AuthenticatedUser,
                new
                {
                    CurrentPassword = "Password123!",
                    NewPassword = "weak_password",
                });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenNoFieldsProvided()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            await DatabaseSeeder.SeedUserAsync(
                db,
                username: "john",
                email: "john@test.com",
                password: "Password123!",
                fullName: "John Smith"
            );

            var token = await _client.LoginAndGetToken("john", "Password123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.AuthenticatedUser,
                new{}
             );

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenRequestBodyIsInvalid()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            await DatabaseSeeder.SeedUserAsync(
                db,
                username: "john",
                email: "john@test.com",
                password: "Password123!",
                fullName: "John Smith"
            );

            var token = await _client.LoginAndGetToken("john", "Password123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.AuthenticatedUser,
                new
                {
                    Pass = "P@ssword123!",
                }
            );

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
