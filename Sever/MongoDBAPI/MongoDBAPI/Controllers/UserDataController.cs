using Microsoft.AspNetCore.Mvc;
using MongoDBAPI.Services;
using MongoDBAPI.Classes;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MongoDBAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[Controller]")]
    public class UserDataController : ControllerBase
    {
        private readonly UserService _userDataService;

        public UserDataController(UserService UserDatasService) =>
            _userDataService = UserDatasService;
        [Route("AuthTest")]
        [HttpGet]
        public async Task<IActionResult> AuthTest()
        {
            return Ok("AuthtokenValid");
        }

        [AllowAnonymous]
        [Route("Login")]
        [HttpPost] //dfalkdjsalkfjdsalkfjdlksajflkdsjaföldsjalkfjdsalkfjdslkajflkdsjafkdsjalkfjdsalkfjdlksajflkdsjalkfdsaujfkdsalkfjdlksajfkldösaj
        public async Task<IActionResult> Login(string username, string plainPassword)
        {
            var token = await _userDataService.Login(username, plainPassword);

            if (token == null)
                return Unauthorized();

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("Register/")]
        public async Task<IActionResult> Register(string username, string plainPassword)
        {
            var privKey = await _userDataService.Register(username, plainPassword);

            if (privKey.Equals(string.Empty))
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(Register), privKey);
        }

        [HttpGet]
        public async Task<List<User>> Get() =>
            await _userDataService.GetAsync();

        [AllowAnonymous]
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<User>> GetByID(string id)
        {
            var UserData = await _userDataService.GetAsync(id);

            if (UserData is null)
            {
                return NotFound();
            }

            return UserData;
        }

        [AllowAnonymous]
        [HttpGet("GetUser/{username}")]
        public async Task<ActionResult<List<User>>> GetByUser(string username)
        {
            var UserData = await _userDataService.GetAsyncByUser(username);

            if (UserData is null)
            {
                return NotFound();
            }

            return UserData;
        }

        [HttpPost]
        public async Task<IActionResult> Post(User newUserData)
        {
            await _userDataService.CreateAsync(newUserData);

            return CreatedAtAction(nameof(Get), new { id = newUserData.Id }, newUserData);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, User updatedUserData)
        {
            var UserData = await _userDataService.GetAsync(id);

            if (UserData is null)
            {
                return NotFound();
            }

            updatedUserData.Id = UserData.Id;

            await _userDataService.UpdateAsync(id, updatedUserData);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var UserData = await _userDataService.GetAsync(id);

            if (UserData is null)
            {
                return NotFound();
            }

            await _userDataService.RemoveAsync(id);

            return NoContent();
        }
    }
}
