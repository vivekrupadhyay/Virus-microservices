using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceMicroservice.Model
{
    public class SwipeType
    {
        public static readonly string DocumentName = "swipetype";
        public Guid Id { get; set; }
        public string SwipeTypeName { get; set; }
    }
}
