using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Middleware
{
    public static class TokenHelper
    {
        public const string App_token = "@<?T<E98Gy,AuA*zdk40nPxIkwW[(l";
        public const string token_issuer = "@<?T<E98Gy,AuA*zdk40nPxIkwW[(l";
        public static Action<AuthenticationOptions> AuthenticationOptions()
        {
            return options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            };
        }
        public static Action<JwtBearerOptions> JwtBearerOptions(bool isDevelopment = false)
        {
            return options =>
            {
                options.RequireHttpsMetadata = !isDevelopment;
                options.SaveToken = true;
                options.TokenValidationParameters = GetTokenValidationParameters(true);
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        string authorization = context.Request.Headers["Authorization"];//"X-Authorization
                        if (string.IsNullOrEmpty(authorization))
                        {
                            context.NoResult();
                            return Task.CompletedTask;
                        }
                        if (authorization.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = authorization.Substring("Bearer".Length).Trim();
                        }
                        if (string.IsNullOrEmpty(context.Token))
                        {
                            context.NoResult();
                            return Task.CompletedTask;
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Fail(context.Exception.Message);
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                            WriteProblemDetails(context);
                        return Task.CompletedTask;
                    }
                };
            };
        }
        public static TokenValidationParameters GetTokenValidationParameters(bool validateLifeTime = true)
        {
            return new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(App_token)),
                ValidateAudience = false,
                ValidIssuer = token_issuer,
                ValidateIssuer=true,
                ValidateIssuerSigningKey=true,
                ValidateLifetime=validateLifeTime,
                LifetimeValidator=CustomLifetimeValidator
            };
        }
        public static bool CustomLifetimeValidator(DateTime? notBefore,DateTime? expire, SecurityToken securityToken,TokenValidationParameters tokenValidationParameters)
        {
            return expire > DateTime.Now;
        }
        public static void WriteProblemDetails(BaseContext<JwtBearerOptions> context)
        {
            var problemDetails = GetProblemDetails(context);
            if (problemDetails.Detail.StartsWith("IDX10230"))
                context.Response.Headers["Token-Expired"] = true.ToString();
            HttpResponse response = context.HttpContext.Response;
            response.ContentType = "application/problem+json";
            response.StatusCode = problemDetails.Status.Value;
            response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
        }
        private static ProblemDetails GetProblemDetails(BaseContext<JwtBearerOptions> context)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Unauthorized",
                Status = (int)HttpStatusCode.Unauthorized,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Instance = context.HttpContext.Request.Path
            };
            if (context is AuthenticationFailedContext)
                problemDetails.Detail = (context as AuthenticationFailedContext).Exception?.Message;
            else if (context is JwtBearerChallengeContext)
                problemDetails.Detail = (context as JwtBearerChallengeContext).AuthenticateFailure?.Message;
            problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
            return problemDetails;
        }
    }
}
