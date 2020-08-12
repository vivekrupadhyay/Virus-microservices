using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendenceMicroservice.Model
{
    public class TimeSheet
    {
        public static readonly string DocumentName = "timesheet";
        public Guid Id { get; set; }
        [Required]
        public List<MonthWeekYear> MonthWeekYearList { get; set; } //Aug1Week#20
        [Required]
        public DateTime FromDate { get; set; }
        [Required]
        public DateTime ToDate { get; set; }
        public Guid UserId { get; set; }

    }
    public class MonthWeekYear
    {
        public Guid Id { get; set; }
        public string Month { get; set; }
        public string Week { get; set; }
        public string Year { get; set; }

    }
}
