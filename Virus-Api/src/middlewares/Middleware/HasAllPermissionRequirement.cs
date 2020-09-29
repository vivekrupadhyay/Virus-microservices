using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Middleware
{
    public class HasAllPermissionRequirement:IAuthorizationRequirement
    {
        public IEnumerable<string> Permission { get; }
        public HasAllPermissionRequirement(IEnumerable<string> permission)
        {
            Permission = permission;
        }
    }
}
