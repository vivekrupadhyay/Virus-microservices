using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Middleware
{
   public  class Authorization
    {
        public static Action<JwtBearerOptions> JwtBearerOptions(IConfigurationSection configuration, bool isDevelopment = false)
        {
            return options =>
            {
                options.RequireHttpsMetadata = !isDevelopment;
                options.SaveToken = true;
                options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
                options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["IssuerSigningKey"]));
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.ValidIssuer = configuration["Issuer"];
                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                options.TokenValidationParameters.ValidateLifetime = true;

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        string authorizeHeader = context.Request.Headers[configuration["AuthorizeHeader"]];
                        if (string.IsNullOrEmpty(authorizeHeader))
                        {
                            context.NoResult();
                            return Task.CompletedTask;
                        }

                        if (authorizeHeader.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = authorizeHeader.Substring("Bearer".Length).Trim();
                        }
                        if (string.IsNullOrEmpty(context.Token))
                        {
                            context.NoResult();
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };

            };
        }
        public static Action<AuthorizationOptions> AuthorizationOptions()
        {
            return options =>
            {
                options.AddPolicy("DeployVirusNonProd", policy => policy.AddRequirements(new HasPermissionRequirement("DeployVirusNonProd")));
                options.AddPolicy("DeployVirusProd", policy => policy.AddRequirements(new HasPermissionRequirement("DeployVirusProd")));

            };
        }
    }
}
