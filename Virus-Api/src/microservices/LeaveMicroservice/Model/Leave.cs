using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveMicroservice.Model
{
    public class Leave
    {
        public static readonly string DocumentName = "leave";
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool isFullDay { get; set; }
        public bool isHalfDay { get; set; }
        public bool isFirstHalf { get; set; }
        public bool isSecondHalf { get; set; }
        public List<LeaveType> leaveType { get; set; }
        public int totalDays { get; set; }
        public string Reason { get; set; }
    }
    public class LeaveType
    {
        public static readonly string DocumentName = "leavetype";
        public Guid Id { get; set; }
        public string TypeName { get; set; }
    }
}
