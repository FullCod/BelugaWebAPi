using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sendeazy.Api.Dtos;
using Sendeazy.Api.Helpers;
using SendeoApi.Helpers;
using SendeoApi.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Dtos;
using WebApi.Entities;


namespace SendeoApi.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/account")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private IUserService _userService;
        private readonly AppSettings _appSettings;
        private readonly ConnectionStrings _connectionStrings;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _env;

        public AccountController(IUserService userService,
                                  IMapper mapper,
                                  IOptions<AppSettings> appSettings,
                                  IOptions<ConnectionStrings> connectionStrings,
                                  IHttpContextAccessor httpContextAccessor,
                                  IHostingEnvironment env)
        {
            _userService = userService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
            _appSettings = appSettings.Value;
            _connectionStrings = connectionStrings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserDto userDto)
        {
            var userFromRepo = await _userService.Authenticate(userDto.UserName, userDto.Password, _connectionStrings.SendyConnectionString);
            if (userFromRepo == null)
            {
                return StatusCode(500, "Identifiant ou mot de passe incorrect");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
              {
            new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
            new Claim(ClaimTypes.Name, userFromRepo.UserName)
              }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var user = _mapper.Map<UserForDetailDto>(userFromRepo);

            return Ok(new
            {
                Token = tokenString,
                user
            });

        }

        [AllowAnonymous]
        [HttpPost("register")]
        [EnableCors("MyPolicy")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();
            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            if (await _userService.UserExists(userToCreate))
            {
                return BadRequest("UserName " + userToCreate.UserName + " is already taken");
            }
            try
            {
                var createdUser = _userService.Create(userToCreate, userForRegisterDto.Password);
                var userToReturn = _mapper.Map<UserForDetailDto>(createdUser);
                return CreatedAtRoute("GetUser", new { id = createdUser.Id }, userToReturn);
            }
            catch (AppException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetById(id);
            return Ok(user);
        }
        [AllowAnonymous]
        [HttpGet("getusers")]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var curentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _userService.GetById(curentUserId);
            userParams.UserId = curentUserId;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }
            var users = await _userService.GetAll(userParams);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

        [HttpGet("FacebookLogin")]
        public IActionResult FacebookLogin()
        {
            var challengeResult = Challenge(new AuthenticationProperties() { RedirectUri = "/" }, "Facebook");
            return challengeResult;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (id != int.Parse(userId))
            {
                return Unauthorized();
            }
            var userFromRepo = await _userService.GetById(id);
            _mapper.Map(userForUpdateDto, userFromRepo);
            var updateid = await _userService.Update(userFromRepo);
            if (updateid > 0)
            {
                return NoContent();
            }
            return BadRequest($"Updating user {id} failed");
        }
    }
}