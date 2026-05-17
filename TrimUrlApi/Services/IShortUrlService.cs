using TrimUrlApi.Entities;
using TrimUrlApi.Models;

namespace TrimUrlApi.Services
{
    public interface IShortUrlService
    {
        Task<ShortUrlGetModel?> GetByCode(string code);
        Task<List<ShortUrlGetModel>> GetByCreatorId(int? id);
        Task<ShortUrl> Create(ShortUrlPostModel postModel, int? userId);
        Task<ShortUrl?> UpdateByCode(string code, ShortUrlPutModel putModel, int? userId);
        Task<ShortUrl?> DeleteByCode(string code, int? userId);
        Task<ShortUrl?> DeleteByCodeAsAdmin(string code);
    }
}
