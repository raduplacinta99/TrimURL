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
        [Fact]
        public async Task Create_ShouldReturnShortUrl_WhenUrlIsValid()
        {
            var repoMock = new Mock<IShortUrlRepository>();
            var service = new ShortUrlService(repoMock.Object);
            var validUrl = "https://google.com";

            var postModel = new ShortUrlPostModel
            {
                Url = validUrl
            };

            var result = await service.Create(postModel, null);

            Assert.NotNull(result);
            Assert.Equal(validUrl, result.Url);
        }

        [Fact]
        public async Task Create_ShouldThrowException_WhenUrlIsInvalid()
        {
            var repoMock = new Mock<IShortUrlRepository>();
            var service = new ShortUrlService(repoMock.Object);
            var invalidUrl = "not-a-url";

            var postModel = new ShortUrlPostModel
            {
                Url = invalidUrl
            };

            await Assert.ThrowsAsync<InvalidUrlStringException>(() =>
                service.Create(postModel, null));
        }
    }
}
