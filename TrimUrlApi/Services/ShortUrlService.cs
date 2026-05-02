using TrimUrlApi.Entities;
using TrimUrlApi.Models;
using TrimUrlApi.Repositories;
using TrimUrlApi.Exceptions;

namespace TrimUrlApi.Services
{
    public class ShortUrlService(ShortUrlRepository suReporitory)
    {
        private readonly ShortUrlRepository _suRepository = suReporitory;
        private static readonly Random _random = new();

        public async Task<ShortUrlGetModel?> GetByCode(string code)
        {
            var shortUrl = await _suRepository.ReadByCode(code);
            if (shortUrl == null)
            {
                throw new ShortUrlNotFoundByCodeException(code);
            }
            if (shortUrl.ExpiresAt < DateTime.Now)
            {
                throw new ShortUrlExpiredException();
            }

            shortUrl.AccessCount++;
            await _suRepository.Update(shortUrl);
            return new ShortUrlGetModel(shortUrl);
        }

        public async Task<List<ShortUrlGetModel>> GetByCreatorId(int? id)
        {
            var shortUrlList = await _suRepository.ReadByCreatorId(id);
            if (id == null || shortUrlList.Count == 0)
            {
                throw new ShortUrlsNotFoundException();
            }

            return shortUrlList.Select(su => new ShortUrlGetModel(su)).ToList();
        }

        public async Task<ShortUrl> Create(ShortUrlPostModel postModel, int? userId)
        {
            if (!IsValidUrl(postModel.Url))
            {
                throw new InvalidUrlStringException(postModel.Url);
            }

            var code = GenerateCode();
            while (await _suRepository.ReadByCode(code) != null)
            {
                code = GenerateCode();
            }

            var shortUrl = new ShortUrl
            {
                CreatorId = userId,
                Url = postModel.Url,
                Code = code,
                ExpiresAt = (postModel.ExpiresAt != DateTime.MaxValue) ? postModel.ExpiresAt : null,
                AccessCount = 0,
            };
            await _suRepository.Create(shortUrl);
            return shortUrl;
        }

        public async Task<ShortUrl?> UpdateByCode(string code, ShortUrlPutModel putModel, int? userId)
        {
            if (!IsValidUrl(putModel.Url))
            {
                throw new InvalidUrlStringException(putModel.Url);
            }

            var shortUrl = await _suRepository.ReadByCode(code);
            if (shortUrl == null)
            {
                throw new ShortUrlNotFoundByCodeException(code);
            }

            if (shortUrl.CreatorId == null || shortUrl.CreatorId != userId)
            {
                throw new ForbiddenShortUrlAccessException();
            }

            shortUrl.Url = putModel.Url;
            if (putModel.ExpiresAt != DateTime.MaxValue)
            {
                shortUrl.ExpiresAt = putModel.ExpiresAt;
            }
            await _suRepository.Update(shortUrl);
            return shortUrl;
        }

        public async Task<ShortUrl?> DeleteByCode(string code, int? userId)
        {
            var shortUrl = await _suRepository.ReadByCode(code);
            if (shortUrl == null)
            {
                throw new ShortUrlNotFoundByCodeException(code);
            }

            if (shortUrl.CreatorId == null || shortUrl.CreatorId != userId)
            {
                throw new ForbiddenShortUrlAccessException();
            }

            await _suRepository.DeleteById(shortUrl.Id);
            return shortUrl;
        }

        public async Task<ShortUrl?> DeleteByCode(string code)
        {
            var shortUrl = await _suRepository.ReadByCode(code);
            if (shortUrl == null)
            {
                throw new ShortUrlNotFoundByCodeException(code);
            }

            await _suRepository.DeleteById(shortUrl.Id);
            return shortUrl;
        }

        public bool IsValidUrl(string url)
        {
            _ = Uri.TryCreate(url, UriKind.Absolute, out var uriResult);
            return uriResult != null && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private static string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Range(0, 6)
                .Select(_ => chars[_random.Next(chars.Length)])
                .ToArray());
        }
    }
}
