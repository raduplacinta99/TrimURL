using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TrimUrlApi.Models;
using TrimUrlApi.Services;
using TrimUrlApi.Extensions;

namespace TrimUrlApi.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController(ILogger<UserController> logger, IUserService userService) : ControllerBase
    {
        private readonly ILogger<UserController> _logger = logger;
        private readonly IUserService _userService = userService;

        [HttpPost()]
        public async Task<IActionResult> Create(UserPostModel postModel)
        {
            var userRespModel = await _userService.Create(postModel);
            return Ok(userRespModel);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetByAuthUsername()
        {
            var username = User.GetAuthUsername();
            var userRespModel = await _userService.GetByUsername(username);
            return Ok(userRespModel);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateByAuthUsername(UserPutModel putModel)
        {
            var username = User.GetAuthUsername();
            var userRespModel = await _userService.UpdateByUsername(username, putModel);
            return Ok(userRespModel);
        }

        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteByAuthUsername()
        {
            var username = User.GetAuthUsername();
            var userRespModel = await _userService.DeleteByUsername(username);
            return NoContent();
        }
    }
}
