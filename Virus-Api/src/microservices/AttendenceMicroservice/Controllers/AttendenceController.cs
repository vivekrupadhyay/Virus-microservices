using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendenceMicroservice.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Middleware;

namespace AttendenceMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendenceController : ControllerBase
    {
        #region Field
        private readonly IAttendenceRepository _attenRepository;
        private readonly IJwtBuilder _jwtBuilder;
        #endregion
        #region Ctor
        public AttendenceController(IAttendenceRepository attenRepository, IJwtBuilder jwtBuilder)
        {
            _attenRepository = attenRepository;
            _jwtBuilder = jwtBuilder;
        }
        #endregion

        #region Controller
        [HttpPost(Name = "clockin")]
        [Authorize(Roles = "User, Admin")]
        public IActionResult ClockIn()
        {
            return Ok();
        }
        [HttpPost(Name = "clockout")]
        [Authorize(Roles = "User, Admin")]
        public IActionResult ClockOut()
        {
            return Ok();
        }
        [HttpGet(Name = "attendencelist")]
        [Authorize(Roles = "User, Admin")]
        public IActionResult AttenenceList()
        {
            return Ok();
        }
        #endregion
    }
}
