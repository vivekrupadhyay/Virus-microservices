using System;
using IdentityMicroservice.Model;
using Middleware;
using IdentityMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace IdentityMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class IdentityController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly JwtOptions _options;
        public IdentityController(IUserRepository userRepository, IJwtBuilder jwtBuilder, IOptions<JwtOptions> options)
        {
            _userRepository = userRepository;
            _jwtBuilder = jwtBuilder;
            _options = options.Value;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] User user, [FromQuery(Name = "email")] string email, [FromQuery(Name = "CompanyCode")] string CompanyCode)
        {
            var userDetails = _userRepository.Login(user.Email, EncryptionLibrary.EncryptText(user.Password), user.CompanyCode);

            if (userDetails == null)
            {
                return NotFound("User not found.");
            }

            if (userDetails.RoleName == "Admin")
            {
                return Ok(_jwtBuilder.GetToken(userDetails.Id,userDetails.RoleName));
            }
            else if (userDetails.RoleName == "User")
            {
                //return Ok(_jwtBuilder.GetToken(userDetails.Id));
                //return Ok(_jwtBuilder.GetToken(userDetails.Id, userDetails.RoleName));
                
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_options.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, userDetails.Id.ToString()),
                    new Claim(ClaimTypes.Role, userDetails.RoleName)
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Token = tokenHandler.WriteToken(token);

                return userDetails.Token;
            }
            else
            {
                return BadRequest("Could not authenticate user.");
            }
            //return Ok(userDetails);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> CancelAccessToken()
        {
            await _jwtBuilder.DeactivateCurrentAsync();

            return NoContent();
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] User user)
        {
            User model = new User();
            var u = _userRepository.GetUser(user.Email);

            if (u != null)
            {
                return BadRequest("User already exists.");
            }
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Email = user.Email;
            model.Password = EncryptionLibrary.EncryptText(user.Password);
            model.Mobile = user.Mobile;
            model.RoleName = user.RoleName;
            model.CompanyCode = user.CompanyCode;
            model.CompanyName = user.CompanyName;
            model.IsActive = user.IsActive;
            model.EnteredBy = user.Id;
            _userRepository.InsertUser(model);

            return Ok();
        }
        [HttpGet("getall")]
        [Authorize(Roles ="User")]
        public ActionResult GetAllUser()
        {
            var users = _userRepository.GetAllUser();


            return Ok(users);
        }

        [HttpGet("validate")]
        public ActionResult<Guid> Validate([FromQuery(Name = "email")] string email, [FromQuery(Name = "token")] string token)
        {
            var u = _userRepository.GetUser(email);

            if (u == null)
            {
                return NotFound("User not found.");
            }

            var userId = _jwtBuilder.ValidateToken(token);

            if (userId != u.Id)
            {
                return BadRequest("Invalid token.");
            }

            return Ok(userId);
        }
        [HttpPut("updateUser")]
        public ActionResult Put([FromBody] User users)
        {
            if (users != null)
            {
                _userRepository.UpdateUser(users);
                return new OkResult();
            }
            return new NoContentResult();
        }
        [HttpDelete("{id}")]
        public ActionResult deleteUser(Guid id)
        {
            _userRepository.DeleteUser(id);
            return new OkResult();
        }
        //[Authorize(Roles = "Admin")]
        [HttpPut("changePassword")]
        public ActionResult change([FromBody] string email, string oldPassword, string newPassword)
        {
            var userDetails = _userRepository.GetUser(email);
            if (userDetails != null)
            {
                _userRepository.ChangePassword(oldPassword, newPassword, userDetails.Id);
                return new OkResult();
            }
            return new NoContentResult();
        }

    }
}
