using System;
using IdentityMicroservice.Model;
using Middleware;
using IdentityMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace IdentityMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtBuilder _jwtBuilder;
        public IdentityController(IUserRepository userRepository, IJwtBuilder jwtBuilder)
        {
            _userRepository = userRepository;
            _jwtBuilder = jwtBuilder;
        }
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
                return Ok(_jwtBuilder.GetToken(userDetails.Id));
            }
            else if (userDetails.RoleName == "User")
            {
                return Ok(_jwtBuilder.GetToken(userDetails.Id));
            }
            else
            {
                return BadRequest("Could not authenticate user.");
            }

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
            model.RoleId = user.RoleId;
            model.RoleName = user.RoleName;
            model.CompanyCode = user.CompanyCode;
            model.CompanyName = user.CompanyName;
            model.IsActive = user.IsActive;
            model.EnteredBy = user.Id;
            _userRepository.InsertUser(model);

            return Ok();
        }
        [HttpGet("getall")]
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
