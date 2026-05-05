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
            if (!await _userService.IsUsernameAvailable(postModel.Username) || !await _userService.IsEmailAvailable(postModel.EmailAddress))
            {
                return BadRequest("Username or email is already in use.");
            }

            var userRespModel = await _userService.Create(postModel);
            return Ok(userRespModel);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetByAuthUsername()
        {
            var username = User.GetAuthUsername();
            var userRespModel = (username != null) ? await _userService.GetByUsername(username) : null;
            if (userRespModel == null)
            {
                return NotFound($"Username not found: {username}");
            }
            return Ok(userRespModel);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateByAuthUsername(UserPutModel putModel)
        {
            if (putModel.Password == null && putModel.EmailAddress == null)
            {
                return BadRequest("At least one field must be provided: (password, emailAddress).");
            }

            var username = User.GetAuthUsername();
            var userRespModel = (username != null) ? await _userService.UpdateByUsername(username, putModel) : null;
            if (userRespModel == null)
            {
                return NotFound($"Username does not exist: {username}");
            }
            return Ok(userRespModel);
        }

        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteByAuthUsername()
        {
            var username = User.GetAuthUsername();
            var userRespModel = (username != null) ? await _userService.DeleteByUsername(username) : null;
            if (userRespModel == null)
            {
                return NotFound($"Username not found: {username}");
            }
            return NoContent();
        }
    }
}
