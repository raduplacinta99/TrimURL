using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TrimUrlApi.Models;
using TrimUrlApi.Services;
using TrimUrlApi.Extensions;

namespace TrimUrlApi.Controllers
{
    [ApiController]
    [Route("short-urls")]
    public class ShortUrlController(ILogger<ShortUrlController> logger, ShortUrlService shortUrlService) : ControllerBase
    {
        private readonly ILogger<ShortUrlController> _logger = logger;
        private readonly ShortUrlService _shortUrlService = shortUrlService;

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
            var creatorId = User.GetAuthUserId();
            var shortUrlList = await _shortUrlService.GetByCreatorId(creatorId);
            return Ok(shortUrlList);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(ShortUrlPostModel postModel)
        {
            if (!_shortUrlService.IsValidUrl(postModel.Url))
            {
                return BadRequest($"Invalid URL string: {postModel.Url}");
            }

            int? creatorId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                creatorId = User.GetAuthUserId();
            }
            var shortUrl = await _shortUrlService.Create(postModel, creatorId);
            return Ok(shortUrl);
        }

        [Authorize]
        [HttpPut("code/{code}")]
        public async Task<IActionResult> UpdateByCode(string code, ShortUrlPutModel putModel)
        {
            if (!_shortUrlService.IsValidUrl(putModel.Url))
            {
                return BadRequest($"Invalid URL string: {putModel.Url}");
            }

            int? creatorId = User.GetAuthUserId();
            var updatedShortUrl = await _shortUrlService.UpdateByCode(code, putModel, creatorId);
            if (updatedShortUrl == null)
            {
                return Unauthorized();
            }
            return Ok(updatedShortUrl);
        }

        [Authorize]
        [HttpDelete("code/{code}")]
        public async Task<IActionResult> DeleteByCode(string code)
        {
            if (await _shortUrlService.GetByCode(code) == null)
            {
                return NotFound($"No URL found with code: {code}");
            }

            int? creatorId = User.GetAuthUserId();
            var deletedUrl = await _shortUrlService.DeleteByCode(code, creatorId);
            if (deletedUrl == null)
            {
                return Unauthorized();
            }
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

            var deletedUrl = await _shortUrlService.DeleteByCode(code);
            if (deletedUrl == null)
            {
                return NotFound($"No URL found with code: {code}");
            }
            return NoContent();
        }
    }
}
