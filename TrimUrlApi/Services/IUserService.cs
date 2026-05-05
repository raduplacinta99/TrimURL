using TrimUrlApi.Entities;
using TrimUrlApi.Models;

namespace TrimUrlApi.Services
{
    public interface IUserService
    {
        Task<UserResponseModel> Create(UserPostModel postModel);
        Task<UserResponseModel?> GetByUsername(string username);
        Task<UserResponseModel?> UpdateByUsername(string username, UserPutModel putModel);
        Task<User?> DeleteByUsername(string username);
        Task<bool> IsUsernameAvailable(string username);
        Task<bool> IsEmailAvailable(string email);
    }
}
