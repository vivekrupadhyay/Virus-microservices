using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Middleware
{
    public class HasPermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }
        public HasPermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
