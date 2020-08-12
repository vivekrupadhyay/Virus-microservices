using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceMicroservice.Model
{
    public class SwipeRequest
    {
        public static readonly string DocumentName = "swipe";
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string SwipeType { get; set; }//Swipe,OD
        public string SwipeCategoty { get; set; }//SwipeReq,WFH
        public string SwiftCode { get; set; }
        public string Mode { get; set; }//In,Out,Both
        public DateTime InTime { get; set; }
        public DateTime OutTime { get; set; }
        public string Reason { get; set; }
        public Guid EnteredBy { get; set; }
        public DateTime WhenEntered { get; set; }
    }
}
