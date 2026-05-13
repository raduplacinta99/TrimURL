using Moq;
using System;
using System.Collections.Generic;
using System.Text;
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
        private const string InvalidUrl = "not-a-url";

        private const string ValidCode = "abc123";
        private const string MissingCode = "missing";


        [Fact]
        public async Task Create_ShouldReturnShortUrl_WhenUrlIsValid()
        {
            var repoMock = new Mock<IShortUrlRepository>();
            var service = new ShortUrlService(repoMock.Object);

            var postModel = new ShortUrlPostModel
            {
                Url = ValidUrl
            };

            repoMock.Setup(r => r.Create(It.IsAny<ShortUrl>())).Returns((ShortUrl s) => Task.FromResult(s));

            var result = await service.Create(postModel, null);

            Assert.NotNull(result);
            Assert.Equal(ValidUrl, result.Url);
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenUrlIsInvalid()
        {
            var repoMock = new Mock<IShortUrlRepository>();
            var service = new ShortUrlService(repoMock.Object);

            var postModel = new ShortUrlPostModel
            {
                Url = InvalidUrl
            };

            await Assert.ThrowsAsync<InvalidUrlStringException>(() =>
                service.Create(postModel, null));
        }

        [Fact]
        public async Task GetByCode_ShouldReturnShortUrl_WhenCodeExists()
        {
            var repoMock = new Mock<IShortUrlRepository>();
            var service = new ShortUrlService(repoMock.Object);

            var shortUrl = new ShortUrl
            {
                Code = ValidCode,
                Url = ValidUrl,
                AccessCount = 0,
            };

            repoMock.Setup(r => r.ReadByCode(ValidCode)).ReturnsAsync(shortUrl);

            var result = await service.GetByCode(ValidCode);

            Assert.NotNull(result);
            Assert.Equal(ValidCode, result.Code);
            Assert.Equal(ValidUrl, result.Url);
            Assert.Equal(1, result.AccessCount);
        }

        [Fact]
        public async Task GetByCode_ShouldThrowException_WhenCodeDoesNotExist()
        {
            var repoMock = new Mock<IShortUrlRepository>();
            var service = new ShortUrlService(repoMock.Object);

            repoMock.Setup(r => r.ReadByCode(MissingCode)).ReturnsAsync((ShortUrl?)null);

            await Assert.ThrowsAsync<ShortUrlNotFoundByCodeException>(() =>
                service.GetByCode(MissingCode));
        }

        [Fact]
        public async Task GetByCode_ShouldThrowException_WhenCodeIsExpired()
        {
            var repoMock = new Mock<IShortUrlRepository>();
            var service = new ShortUrlService(repoMock.Object);

            var shortUrl = new ShortUrl
            {
                Code = ValidCode,
                Url = ValidUrl,
                ExpiresAt = DateTime.Parse("January 15, 2000")
            };

            repoMock.Setup(r => r.ReadByCode(ValidCode)).ReturnsAsync(shortUrl);

            await Assert.ThrowsAsync<ShortUrlExpiredException>(() =>
                service.GetByCode(ValidCode));
        }

        [Fact]
        public async Task GetByCreatorId_ShouldReturnShortUrls_WhenCreatorExists()
        {
            var repoMock = new Mock<IShortUrlRepository>();
            var service = new ShortUrlService(repoMock.Object);
            var validCreatorId = 1;

            var shortUrlList = new List<ShortUrl> {
                new() {
                    Code = "def456",
                    Url = "https://www.wikipedia.org/",
                    CreatorId = validCreatorId
                },
                new() {
                    Code = "ghi789",
                    Url = "https://www.youtube.com/",
                    CreatorId = validCreatorId
                },

            };

            repoMock.Setup(r => r.ReadByCreatorId(validCreatorId)).ReturnsAsync(shortUrlList);

            var result = await service.GetByCreatorId(validCreatorId);

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
            var repoMock = new Mock<IShortUrlRepository>();
            var service = new ShortUrlService(repoMock.Object);
            var invalidCreatorId = 2;

            repoMock.Setup(r => r.ReadByCreatorId(invalidCreatorId)).ReturnsAsync((List<ShortUrl>)[]);

            await Assert.ThrowsAsync<ShortUrlsNotFoundException>(() =>
                service.GetByCreatorId(invalidCreatorId));
        }
    }
}
