using Microsoft.EntityFrameworkCore;
using TrimUrlApi.Database;
using TrimUrlApi.Entities;

namespace TrimUrlApi.Repositories
{
    public class UserRepository(MainDbContext dbContext) : BaseRepository<User>(dbContext), IUserRepository
    {
        private readonly MainDbContext _dbContext = dbContext;
        private readonly DbSet<User> _dbSet = dbContext.Set<User>();

        public async Task<User?> ReadByUsername(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }
        public async Task<User?> ReadByEmail(string emailAddress)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.EmailAddress.ToLower() == emailAddress.ToLower());
        }
    }
}
