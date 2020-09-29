using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceMicroservice.Model
{
    public class SwipeCategoty
    {
        public static readonly string DocumentName = "swipecategory";
        public Guid Id { get; set; }
        public string SwipeCategoryName { get; set; }//SwipeReq,WFH
    }
}
