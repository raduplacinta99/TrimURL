using Moq;
using TrimUrlApi.Entities;
using TrimUrlApi.Exceptions;
using TrimUrlApi.Models;
using TrimUrlApi.Repositories;
using TrimUrlApi.Services;

namespace TrimUrlApi.Tests.Services
{
    public class ShortUrlServiceTests
    {
        private const string ValidUrl = "https://google.com";
        private const string ValidUpdateUrl = "https://bing.com";
        private const string InvalidUrl = "not-a-url";

        private const string ValidCode = "abc123";
        private const string MissingCode = "missing";

        private const int ValidCreatorId = 1;
        private const int InvalidCreatorId = 2;

        private readonly Mock<IShortUrlRepository> _repoMock;
        private readonly ShortUrlService _service;

        public ShortUrlServiceTests()
        {
            _repoMock = new Mock<IShortUrlRepository>();
            _service = new ShortUrlService(_repoMock.Object);
        }


        [Fact]
        public async Task Create_ShouldReturnShortUrl_WhenUrlIsValid()
        {
            var postModel = new ShortUrlPostModel
            {
                Url = ValidUrl
            };

            _repoMock.Setup(r => r.Create(It.IsAny<ShortUrl>())).Returns((ShortUrl s) => Task.FromResult(s));

            var result = await _service.Create(postModel, null);

            Assert.NotNull(result);
            Assert.Equal(ValidUrl, result.Url);
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenUrlIsInvalid()
        {
            var postModel = new ShortUrlPostModel
            {
                Url = InvalidUrl
            };

            await Assert.ThrowsAsync<InvalidUrlStringException>(() =>
                _service.Create(postModel, null));
        }

        [Fact]
        public async Task GetByCode_ShouldReturnShortUrl_WhenCodeExists()
        {
            var shortUrl = new ShortUrl
            {
                Code = ValidCode,
                Url = ValidUrl,
                AccessCount = 0,
            };

            _repoMock.Setup(r => r.ReadByCode(ValidCode)).ReturnsAsync(shortUrl);

            var result = await _service.GetByCode(ValidCode);

            Assert.NotNull(result);
            Assert.Equal(ValidCode, result.Code);
            Assert.Equal(ValidUrl, result.Url);
            Assert.Equal(1, result.AccessCount);
        }

        [Fact]
        public async Task GetByCode_ShouldThrowException_WhenCodeDoesNotExist()
        {
            _repoMock.Setup(r => r.ReadByCode(MissingCode)).ReturnsAsync((ShortUrl?)null);

            await Assert.ThrowsAsync<ShortUrlNotFoundByCodeException>(() =>
                _service.GetByCode(MissingCode));
        }

        [Fact]
        public async Task GetByCode_ShouldThrowException_WhenCodeIsExpired()
        {
            var shortUrl = new ShortUrl
            {
                Code = ValidCode,
                Url = ValidUrl,
                ExpiresAt = DateTime.Parse("January 15, 2000")
            };

            _repoMock.Setup(r => r.ReadByCode(ValidCode)).ReturnsAsync(shortUrl);

            await Assert.ThrowsAsync<ShortUrlExpiredException>(() =>
                _service.GetByCode(ValidCode));
        }

        [Fact]
        public async Task GetByCreatorId_ShouldReturnShortUrls_WhenCreatorExists()
        {
            var shortUrlList = new List<ShortUrl> {
                new() {
                    Code = "def456",
                    Url = "https://www.wikipedia.org/",
                    CreatorId = ValidCreatorId
                },
                new() {
                    Code = "ghi789",
                    Url = "https://www.youtube.com/",
                    CreatorId = ValidCreatorId
                },

            };

            _repoMock.Setup(r => r.ReadByCreatorId(ValidCreatorId)).ReturnsAsync(shortUrlList);

            var result = await _service.GetByCreatorId(ValidCreatorId);

            Assert.NotNull(result);
            Assert.Equal(shortUrlList.Count, result.Count);

            foreach (var expected in shortUrlList)
            {
                Assert.Contains(result, actual =>
                    actual.Code == expected.Code &&
                    actual.Url == expected.Url);
            }
        }

        [Fact]
        public async Task GetByCreatorId_ShouldThrowException_WhenCreatorDoesNotExist()
        {
            _repoMock.Setup(r => r.ReadByCreatorId(InvalidCreatorId)).ReturnsAsync((List<ShortUrl>)[]);

            await Assert.ThrowsAsync<ShortUrlsNotFoundException>(() =>
                _service.GetByCreatorId(InvalidCreatorId));
        }

        [Fact]
        public async Task UpdateByCode_ShouldReturnShortUrl_WhenCodeExists()
        {
            var shortUrl = new ShortUrl
            {
                Code = ValidCode,
                Url = ValidUrl,
                CreatorId = ValidCreatorId,
            };

            var putModel = new ShortUrlPutModel
            {
                Url = ValidUpdateUrl
            };

            var updatedUrl = new ShortUrl
            {
                Code = shortUrl.Code,
                Url = putModel.Url,
                CreatorId = shortUrl.CreatorId
            };

            _repoMock.Setup(r => r.ReadByCode(ValidCode)).ReturnsAsync(shortUrl);
            _repoMock.Setup(r => r.Update(It.IsAny<ShortUrl>())).Returns((ShortUrl s) => Task.FromResult(s));


            var result = await _service.UpdateByCode(ValidCode, putModel, ValidCreatorId);

            Assert.NotNull(result);
            Assert.Equal(shortUrl.Code, result.Code);
            Assert.Equal(putModel.Url, result.Url);
            Assert.Equal(shortUrl.CreatorId, result.CreatorId);
        }

        [Fact]
        public async Task UpdateByCode_ShouldThrowException_WhenCodeDoesNotExist()
        {
            var putModel = new ShortUrlPutModel
            {
                Url = ValidUpdateUrl
            };

            _repoMock.Setup(r => r.ReadByCode(MissingCode)).ReturnsAsync((ShortUrl?)null);
            _repoMock.Setup(r => r.Update(It.IsAny<ShortUrl>())).Returns((ShortUrl s) => Task.FromResult(s));

            await Assert.ThrowsAsync<ShortUrlNotFoundByCodeException>(() =>
                _service.UpdateByCode(MissingCode, putModel, ValidCreatorId));
        }

        [Fact]
        public async Task UpdateByCode_ShouldThrowException_WhenUrlIsInvalid()
        {
            var putModel = new ShortUrlPutModel
            {
                Url = InvalidUrl
            };

            await Assert.ThrowsAsync<InvalidUrlStringException>(() =>
                _service.UpdateByCode(ValidCode, putModel, ValidCreatorId));
        }

        [Fact]
        public async Task UpdateByCode_ShouldThrowException_WhenInvalidCreatorId()
        {
            var shortUrl = new ShortUrl
            {
                Code = ValidCode,
                Url = ValidUrl,
                CreatorId = ValidCreatorId,
            };

            var putModel = new ShortUrlPutModel
            {
                Url = ValidUpdateUrl
            };

            _repoMock.Setup(r => r.ReadByCode(ValidCode)).ReturnsAsync(shortUrl);
            _repoMock.Setup(r => r.Update(It.IsAny<ShortUrl>())).Returns((ShortUrl s) => Task.FromResult(s));

            await Assert.ThrowsAsync<ForbiddenShortUrlAccessException>(() =>
                _service.UpdateByCode(ValidCode, putModel, InvalidCreatorId));
        }

        [Fact]
        public async Task DeleteByCode_ShouldReturnShortUrl_WhenCodeExists()
        {
            var validId = 1;

            var shortUrl = new ShortUrl
            {
                Id = validId,
                Code = ValidCode,
                Url = ValidUrl,
                CreatorId = ValidCreatorId,
            };

            _repoMock.Setup(r => r.ReadByCode(ValidCode)).ReturnsAsync(shortUrl);
            _repoMock.Setup(r => r.DeleteById(It.IsAny<int>())).Returns(Task.CompletedTask);

            var result = await _service.DeleteByCode(ValidCode, ValidCreatorId);

            Assert.NotNull(result);
            Assert.Equal(shortUrl.Id, result.Id);
            Assert.Equal(shortUrl.Code, result.Code);
            Assert.Equal(shortUrl.Url, result.Url);
            Assert.Equal(shortUrl.CreatorId, result.CreatorId);
        }

        [Fact]
        public async Task DeleteByCode_ShouldThrowException_WhenCodeDoesNotExist()
        {
            _repoMock.Setup(r => r.ReadByCode(MissingCode)).ReturnsAsync((ShortUrl?)null);
            _repoMock.Setup(r => r.DeleteById(It.IsAny<int>())).Returns(Task.CompletedTask);

            await Assert.ThrowsAsync<ShortUrlNotFoundByCodeException>(() =>
                _service.DeleteByCode(MissingCode, ValidCreatorId));
        }

        [Fact]
        public async Task DeleteByCode_ShouldThrowException_WhenInvalidCreatorId()
        {
            var shortUrl = new ShortUrl
            {
                Code = ValidCode,
                Url = ValidUrl,
                CreatorId = ValidCreatorId,
            };

            _repoMock.Setup(r => r.ReadByCode(ValidCode)).ReturnsAsync(shortUrl);
            _repoMock.Setup(r => r.DeleteById(It.IsAny<int>())).Returns(Task.CompletedTask);

            await Assert.ThrowsAsync<ForbiddenShortUrlAccessException>(() =>
                _service.DeleteByCode(ValidCode, InvalidCreatorId));
        }

        [Fact]
        public async Task DeleteByCodeAsAdmin_ShouldReturnShortUrl_WhenCodeExists()
        {
            var validId = 1;

            var shortUrl = new ShortUrl
            {
                Id = validId,
                Code = ValidCode,
                Url = ValidUrl,
            };

            _repoMock.Setup(r => r.ReadByCode(ValidCode)).ReturnsAsync(shortUrl);
            _repoMock.Setup(r => r.DeleteById(It.IsAny<int>())).Returns(Task.CompletedTask);

            var result = await _service.DeleteByCodeAsAdmin(ValidCode);

            Assert.NotNull(result);
            Assert.Equal(shortUrl.Id, result.Id);
            Assert.Equal(shortUrl.Code, result.Code);
            Assert.Equal(shortUrl.Url, result.Url);
        }

        [Fact]
        public async Task DeleteByCodeAsAdmin_ShouldThrowException_WhenCodeDoesNotExist()
        {
            _repoMock.Setup(r => r.ReadByCode(MissingCode)).ReturnsAsync((ShortUrl?)null);
            _repoMock.Setup(r => r.DeleteById(It.IsAny<int>())).Returns(Task.CompletedTask);

            await Assert.ThrowsAsync<ShortUrlNotFoundByCodeException>(() =>
                _service.DeleteByCodeAsAdmin(MissingCode));
        }
    }
}
