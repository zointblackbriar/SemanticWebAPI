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

        //private IAuth _auth;
        //private ITokenManager _jwtManager;
        private IUserService _userService;
        //private IMapper _mapper;
        //private readonly AppSettings _appSettings;
        //private ITokenManager _jwtManager;

        //ITokenManager jwtManager
        public UsersController(IUserService userService)
        {
            _userService = userService;
            //_mapper = mapper;
            //_appSettings = appSettings.Value;
            //_jwtManager = jwtManager;
        }



       //GET: api/values
       [AllowAnonymous]
       [HttpPost("authenticate")]
       //public IActionResult Get([FromForm] SemanticAPI.CredentialModel.AuthCredentials authCreds)
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
