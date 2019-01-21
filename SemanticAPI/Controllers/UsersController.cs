using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
//using SemanticAPI.Auth;
using SemanticAPI.Entities;
using SemanticAPI.Helpers;
using SemanticAPI.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAPI.Controllers
{
    [ApiController]
    [Authorize]
    //[AllowAnonymous]
    [Route("[controller]")]
    public class UsersController : Controller
    {

        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }



       //GET: api/values
       [AllowAnonymous]
       [HttpPost("authenticate")]
       public async Task<IActionResult> Authenticate([FromBody]User userParam)
       {
            var user = await _userService.Authenticate(userParam.UserName, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            return Ok(user);

       }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }



    }
}
