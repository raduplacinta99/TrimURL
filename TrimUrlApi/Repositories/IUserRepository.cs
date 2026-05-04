using TrimUrlApi.Entities;

namespace TrimUrlApi.Repositories
{
    public interface IUserRepository
    {
        Task Create(User user);
        Task<User?> ReadById(int id);
        Task<List<User>?> ReadAll();
        Task Update(User user);
        Task DeleteById(int id);

        Task<User?> ReadByUsername(string username);
        Task<User?> ReadByEmail(string emailAddress);
    }
}
