using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Operations;
using Fithub.Classes;
using MongoDBAPI.Services;
using System.Security.Claims;
using MongoDBAPI.Classes;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SocialManagementController : Controller
    {
        private readonly SocialManagementService _socialManagementController;
        private readonly UserService _userService;

        public SocialManagementController(SocialManagementService socialManagementService, UserService userService)
        {
            _socialManagementController = socialManagementService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<List<SocialManagement>> Get() =>
            await _socialManagementController.GetAsync();

        [AllowAnonymous]
        [HttpPost("{UserId}")]
        public async Task<SocialManagement> GetFromUser(string UserId) =>
           await _socialManagementController.GetByUser(UserId);

        [HttpPut("Friends/SendRequest/{sendToUserId}")]
        public async Task<IActionResult> SendFriendRequest(string sendToUserId)
        {
            string currentUserID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            User userAccept = await _userService.GetAsync(currentUserID);
            User userSent = await _userService.GetAsync(sendToUserId);

            if (userAccept is null || userSent is null)
            {
                return NotFound();
            }

            if (!await _socialManagementController.SendFriendRequest(userAccept.Id, userSent.Id))
            {
                return Conflict();
            }

            return Ok();
        }

        [HttpPut("Friends/AcceptRequest/{userWhoSentId}")]
        public async Task<IActionResult> AcceptFriendRequest(string userWhoSentId)
        {
            string currentUserID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            User userAccept = await _userService.GetAsync(currentUserID);
            User userSent = await _userService.GetAsync(userWhoSentId);

            if (userAccept is null || userSent is null)
            {
                return NotFound();
            }

            await _socialManagementController.AcceptFriendRequest(userAccept, userSent);

            return Ok();
        }

        [HttpGet("Self")]
        public async Task<ActionResult<SocialManagement>> GetSelf()
        {
            string currentUserID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var sm = await _socialManagementController.GetByUser(currentUserID);

            if (sm is null)
            {
                return NotFound();
            }

            return sm;
        }

        [HttpDelete("Friends/RemoveRequest/{requestedId}")]
        public async Task<IActionResult> DeleteRequest(string requestedId)
        {
            string currentUserID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var sm = await _socialManagementController.DeleteFriendRequest(currentUserID, requestedId);

            if (!sm)
            {
                return NotFound();
            }

            return Ok();
        }
        
        [HttpDelete("Friends/Remove/{friendId}")]
        public async Task<IActionResult> RemoveFromFriends(string requestedId)
        {
            string currentUserID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var sm = await _socialManagementController.RemoveFromFriends(currentUserID, requestedId);

            if (!sm)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
