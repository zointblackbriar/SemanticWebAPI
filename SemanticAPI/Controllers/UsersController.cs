using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using SemanticAPI.Auth;
using SemanticAPI.Entities;
using SemanticAPI.Services;


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
       public IActionResult Authenticate([FromBody]User userParam)
       {
            var user = _userService.Authenticate(userParam.UserName, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            return Ok(user);
       }
        
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }


    }
}
