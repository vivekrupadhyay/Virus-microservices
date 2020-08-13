using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceMicroservice.Model
{
    public class Attendence
    {
        public static readonly string DocumentName = "attendence";
        [Required]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string CompanyCode { get; set; }
        public string CurrentAddress { get; set; }
        public string Remark { get; set; }
        public string AttendencePic { get; set; }
        public string IPAddress { get; set; }
        public string Latitude { get; set; }
        public string longitude { get; set; }
        public DateTime? ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public bool IsSortTime { get; set; }
        public bool IsSwipePending { get; set; }
        public bool IsSwipeApproved { get; set; }
        public Guid? EnteredBy { get; set; }
        
    }
}
