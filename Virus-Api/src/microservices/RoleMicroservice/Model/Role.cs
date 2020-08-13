using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoleMicroservice.Model
{
    public class Role
    {
        public static readonly string DocumentName = "roles";
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}
