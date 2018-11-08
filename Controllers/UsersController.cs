using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using demoBusinessReport.Dtos;
using demoBusinessReport.Entities;
using demoBusinessReport.Helpers;
using demoBusinessReport.Services;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace demoBusinessReport.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private UserManager<IdentityUser> _userManagerService;
        private SignInManager<IdentityUser> _signInManagerService;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            UserManager<IdentityUser> userManagerService,
            SignInManager<IdentityUser> signInManagerService)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _userManagerService = userManagerService;
            _signInManagerService = signInManagerService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserDto userDto)
        {
            try
            {
                IdentityUser user = new IdentityUser(userDto.Username);
                IdentityResult result = await _userManagerService.CreateAsync(user, userDto.Password);
                if (result.Succeeded)
                {
                    return Ok(user);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody]UserDto userDto)
        {

                var result = await _signInManagerService.PasswordSignInAsync(userDto.Username, userDto.Password, false, false);

                if (result.Succeeded)
                {
                    var user = await _userManagerService.FindByNameAsync(userDto.Username);
                    var tokenHandler = new JwtSecurityTokenHandler();

                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                                new Claim(ClaimTypes.Name, user.Id.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    // return basic user info (without password) and token to store client side
                    return Ok(new
                    {
                        appUser = user,
                        Token = tokenString
                    });

                }

            return BadRequest();

        }

        #region - sample authenticate
        //[AllowAnonymous]
        //[HttpPost("authenticate")]
        //public IActionResult Authenticate([FromBody]UserDto userDto)
        //{
        //    var user = _userService.Authenticate(userDto.Username, userDto.Password);

        //    if (user == null)
        //        return BadRequest(new { message = "Username or password is incorrect" });

        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //            new Claim(ClaimTypes.Name, user.Id.ToString())
        //        }),
        //        Expires = DateTime.UtcNow.AddDays(7),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    var tokenString = tokenHandler.WriteToken(token);

        //    // return basic user info (without password) and token to store client side
        //    return Ok(new
        //    {
        //        Id = user.Id,
        //        Username = user.Username,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Token = tokenString
        //    });
        //}
        #endregion

        #region - acount CRUD
        //    [HttpGet]
        //    public IActionResult GetAll()
        //    {
        //        var users = _userService.GetAll();
        //        var userDtos = _mapper.Map<IList<UserDto>>(users);
        //        return Ok(userDtos);
        //    }

        //    [HttpGet("{id}")]
        //    public IActionResult GetById(int id)
        //    {
        //        var user = _userService.GetById(id);
        //        var userDto = _mapper.Map<UserDto>(user);
        //        return Ok(userDto);
        //    }

        //    [HttpPut("{id}")]
        //    public IActionResult Update(int id, [FromBody]UserDto userDto)
        //    {
        //        // map dto to entity and set id
        //        var user = _mapper.Map<User>(userDto);
        //        user.Id = id;

        //        try
        //        {
        //            // save 
        //            _userService.Update(user, userDto.Password);
        //            return Ok();
        //        }
        //        catch (AppException ex)
        //        {
        //            // return error message if there was an exception
        //            return BadRequest(new { message = ex.Message });
        //        }
        //    }

        //    [HttpDelete("{id}")]
        //    public IActionResult Delete(int id)
        //    {
        //        _userService.Delete(id);
        //        return Ok();
        //    }
        #endregion
    }
}