using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SemanticAPI.Auth;
using SemanticAPI.Helpers;
using SemanticAPI.Models.Options;
using SemanticAPI.Dtos;
using SemanticAPI.Entities;
using SemanticAPI.Services;
using SemanticAPI.Entities;


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
        private ITokenManager _jwtManager;


        public UsersController(IUserService userService, ITokenManager jwtManager)
        {
            _userService = userService;
            //_mapper = mapper;
            //_appSettings = appSettings.Value;
            _jwtManager = jwtManager;
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
