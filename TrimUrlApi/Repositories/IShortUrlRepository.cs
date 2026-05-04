using TrimUrlApi.Entities;

namespace TrimUrlApi.Repositories
{
    public interface IShortUrlRepository
    {
        Task Create(ShortUrl shortUrl);
        Task<ShortUrl?> ReadById(int id);
        Task<List<ShortUrl>?> ReadAll();
        Task Update(ShortUrl shortUrl);
        Task DeleteById(int id);

        Task<ShortUrl?> ReadByCode(string code);
        Task<List<ShortUrl>> ReadByCreatorId(int? creatorId);
    }
}
