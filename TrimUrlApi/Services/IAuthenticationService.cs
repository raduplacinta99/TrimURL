using TrimUrlApi.Entities;
using TrimUrlApi.Models;

namespace TrimUrlApi.Services
{
    public interface IAuthenticationService
    {
        Task<User?> GetUserByCredentials(LoginPostModel loginModel);
        string GenerateJwtToken(User user);

    }
}
