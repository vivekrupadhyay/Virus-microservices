using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AttendenceMicroservice.Model;
using AttendenceMicroservice.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Middleware;

namespace AttendenceMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [HttpPost("clockin")]
        [Authorize(Roles = "User, Admin")]
        public IActionResult ClockIn([FromBody] Attendence attendence)//, [FromQuery(Name = "userId")] string userId, [FromQuery(Name = "CompanyCode")] string CompanyCode)
        {
            Attendence model = new Attendence();
            if (attendence != null)
            {
                model.UserId = attendence.UserId;
                model.ClockIn = DateTime.SpecifyKind(attendence.ClockIn, DateTimeKind.Local);
                model.ClockOut = DateTime.SpecifyKind(attendence.ClockOut, DateTimeKind.Local);
                model.CompanyCode = attendence.CompanyCode;
                model.CurrentAddress = attendence.CurrentAddress;
                model.AttendencePic = attendence.AttendencePic;
                model.EnteredBy = attendence.EnteredBy;
                model.IPAddress = attendence.IPAddress;
                model.IsSortTime = attendence.IsSortTime;
                model.IsSwipeApproved = attendence.IsSwipeApproved;
                model.IsSwipePending = attendence.IsSwipePending;
                model.Latitude = attendence.Latitude;
                model.longitude = attendence.longitude;
                model.Remark = attendence.Remark;
                _attenRepository.ClockIn(attendence);
            }
            
            return Ok();
        }
        [HttpPost("clockout")]
        [Authorize(Roles = "User, Admin")]
        public IActionResult ClockOut([FromBody] Attendence attendence, [FromQuery(Name = "userId")] string userId, [FromQuery(Name = "CompanyCode")] string CompanyCode)
        {
            Attendence model = new Attendence();
            if (attendence != null)
            {
                model.UserId = attendence.UserId;
                model.ClockIn = DateTime.SpecifyKind(attendence.ClockIn, DateTimeKind.Local);
                model.ClockOut = DateTime.SpecifyKind(attendence.ClockOut, DateTimeKind.Local);
                model.CompanyCode = attendence.CompanyCode;
                model.CurrentAddress = attendence.CurrentAddress;
                model.AttendencePic = attendence.AttendencePic;
                model.EnteredBy = attendence.EnteredBy;
                model.IPAddress = attendence.IPAddress;
                model.IsSortTime = attendence.IsSortTime;
                model.IsSwipeApproved = attendence.IsSwipeApproved;
                model.IsSwipePending = attendence.IsSwipePending;
                model.Latitude = attendence.Latitude;
                model.longitude = attendence.longitude;
                model.Remark = attendence.Remark;
                _attenRepository.ClockOut(model);
            }

            return Ok();
        }
        [HttpGet("attendencelist")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> AttenenceList()
        {
            var result = await Task.FromResult(_attenRepository.GetAll());
            return Ok(result);
        }
        #endregion
    }
}
