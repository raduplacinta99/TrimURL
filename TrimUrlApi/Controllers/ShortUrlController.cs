using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TrimUrlApi.Models;
using TrimUrlApi.Services;
using TrimUrlApi.Extensions;

namespace TrimUrlApi.Controllers
{
    [ApiController]
    [Route("short-urls")]
    public class ShortUrlController(ILogger<ShortUrlController> logger, IShortUrlService shortUrlService) : ControllerBase
    {
        private readonly ILogger<ShortUrlController> _logger = logger;
        private readonly IShortUrlService _shortUrlService = shortUrlService;

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var getModel = await _shortUrlService.GetByCode(code);
            return Ok(getModel);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetByCreatorId()
        {
            var userId = User.GetAuthUserId();
            var shortUrlList = await _shortUrlService.GetByCreatorId(userId);
            return Ok(shortUrlList);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(ShortUrlPostModel postModel)
        {
            int? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                userId = User.GetAuthUserId();
            }
            var shortUrl = await _shortUrlService.Create(postModel, userId);
            return Ok(shortUrl);
        }

        [Authorize]
        [HttpPut("code/{code}")]
        public async Task<IActionResult> UpdateByCode(string code, ShortUrlPutModel putModel)
        {
            int? userId = User.GetAuthUserId();
            var updatedShortUrl = await _shortUrlService.UpdateByCode(code, putModel, userId);
            return Ok(updatedShortUrl);
        }

        [Authorize]
        [HttpDelete("code/{code}")]
        public async Task<IActionResult> DeleteByCode(string code)
        {
            int? userId = User.GetAuthUserId();
            await _shortUrlService.DeleteByCode(code, userId);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("admin/code/{code}")]
        public async Task<IActionResult> DeleteByCodeAsAdmin(string code)
        {
            if (!User.HasAdminPrivileges())
            {
                return Unauthorized();
            }

            await _shortUrlService.DeleteByCodeAsAdmin(code);
            return NoContent();
        }
    }
}
