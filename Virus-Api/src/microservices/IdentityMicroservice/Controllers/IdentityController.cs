using System;
using IdentityMicroservice.Model;
using Middleware;
using IdentityMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroservice.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEncryptor _encryptor;
        public IdentityController(IUserRepository userRepository, IJwtBuilder jwtBuilder, IEncryptor encryptor)
        {
            _userRepository = userRepository;
            _jwtBuilder = jwtBuilder;
            _encryptor = encryptor;
        }
        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] User user, [FromQuery(Name = "d")] string destination = "frontend")
        {
            var u = _userRepository.GetUser(user.Email);

            if (u == null)
            {
                return NotFound("User not found.");
            }

            if (destination == "backend" && !u.IsAdmin)
            {
                return BadRequest("Could not authenticate user.");
            }

            var isValid = u.ValidatePassword(user.Password, _encryptor);

            if (!isValid)
            {
                return BadRequest("Could not authenticate user.");
            }

            var token = _jwtBuilder.GetToken(u.Id);

            return Ok(token);
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] User user)
        {
            var u = _userRepository.GetUser(user.Email);

            if (u != null)
            {
                return BadRequest("User already exists.");
            }

            user.SetPassword(user.Password, _encryptor);
            _userRepository.InsertUser(user);

            return Ok();
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
    }
}
