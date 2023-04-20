using API.Services;
using Fithub.Classes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class SocialPostController : Controller
    {
        private readonly SocialPostService _SocialPostService;
        private readonly SocialManagementService _socialManagementService;

        public SocialPostController(SocialPostService socialPostService, SocialManagementService socialManagementService)
        {
            _SocialPostService = socialPostService;
            _socialManagementService = socialManagementService;
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            string currentUserID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            SocialPost post = await _SocialPostService.GetById(id);


            if (post is null)
            {
                return NotFound();
            }
            if (!post.PostedBy.Equals(currentUserID))
            {
                return Unauthorized();
            }

            await _SocialPostService.Delete(id);

            return Ok();
        }

        //ToDo: Remove this function later
        [AllowAnonymous]
        [HttpGet]
        public async Task<List<SocialPost>> Get() =>
            await _SocialPostService.GetAsync();


        [HttpGet("GetManagement")]
        public async Task<List<SocialManagement>> Gets() =>
            await _socialManagementService.GetAsync();


        [HttpGet("GetByUser")]
        public async Task<ActionResult<List<SocialPost>>> GetByUser(string id)
        {
            string currentUserID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            List<SocialPost> socialPosts = await _SocialPostService.GetAllPostsFromUser(id);

            if (socialPosts is null)
            {
                return NotFound();
            }

            if (socialPosts.Count != 0 && !((await _socialManagementService.AreFriends(socialPosts[0].PostedBy, currentUserID))
                || currentUserID.Equals(socialPosts[0].PostedBy)))
            {
                return Unauthorized();
            }
            return Ok(socialPosts);
        }

        [HttpGet("newest")]
        public async Task<List<SocialPost>> GetNewestPosts()
        {
            string currentUserID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            List<string> allFriends = await _socialManagementService.GetFriends(currentUserID);

            return await _SocialPostService.GetPostsOlderThan(allFriends, DateTime.Now);
        }

        [HttpGet("newest/{dateTime}")]
        public async Task<List<SocialPost>> GetNewestPosts(DateTime dateTime)
        {
            string currentUserID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            List<string> allFriends = await _socialManagementService.GetFriends(currentUserID);

            return await _SocialPostService.GetPostsOlderThan(allFriends, dateTime);
        }

        [HttpPost("NewPost/")]
        public async Task<IActionResult> NewPost([FromBody] SocialPost post)
        {
            post.PostedBy = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            post.DateTime = DateTime.UtcNow;
            await _SocialPostService.NewSocialPost(post);

            return StatusCode(201);
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            //actionContext
            return await _SocialPostService.UploadFile(this.HttpContext.Request, userId);
        }

        [HttpGet("GetFile/{requestPath}")]
        public async Task<IActionResult> GetFile(string requestPath)
        {
            try
            {
                string currentUserID = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                string[] splittedPath = requestPath.Split('\\');
                string ownerOfFile = splittedPath[splittedPath.Length - 3];

                if (currentUserID.Equals(ownerOfFile) || await _socialManagementService.AreFriends(currentUserID, ownerOfFile))
                {

                    (string, byte[]) typeAndFile = await _SocialPostService.GetFile(requestPath);

                    Response.Headers.Add("Content-Type", typeAndFile.Item1);
                    
                    return StatusCode(200, new MemoryStream(typeAndFile.Item2));

                }
                else
                {
                    return Unauthorized();
                }
               
            }

            catch (Exception ex)
            {
                Response.StatusCode = BadRequest().StatusCode;
                return Ok(ex.Message);
            }
        }

        [HttpGet("json")]
        public string GetCoreJson()
        {
            var brdDoc = new SocialPost()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                KindOfPost = KindOfPost.Workout,
                // Media = new byte[] { 2, 232, 231, 231, 234 },
                PostedBy = ObjectId.GenerateNewId().ToString(),
                DateTime = DateTime.UtcNow
            };

            var bsonDocument = brdDoc.ToBsonDocument();
            var jsonDocument = bsonDocument.ToJson();

            return jsonDocument;
        }
    }
}
