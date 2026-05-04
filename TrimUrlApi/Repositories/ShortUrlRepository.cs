using Microsoft.EntityFrameworkCore;
using TrimUrlApi.Database;
using TrimUrlApi.Entities;

namespace TrimUrlApi.Repositories
{
    public class ShortUrlRepository(MainDbContext dbContext) : BaseRepository<ShortUrl>(dbContext), IShortUrlRepository
    {
        private readonly MainDbContext _dbContext = dbContext;
        private readonly DbSet<ShortUrl> _dbSet = dbContext.Set<ShortUrl>();

        public async Task<ShortUrl?> ReadByCode(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(su => su.Code == code);
        }
        public async Task<List<ShortUrl>> ReadByCreatorId(int? creatorId)
        {
            return await _dbSet.Where(su => su.CreatorId == creatorId).ToListAsync();
        }
    }
}
