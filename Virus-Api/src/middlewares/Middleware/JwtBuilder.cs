using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Middleware
{
    public class JwtBuilder : IJwtBuilder
    {
        private readonly JwtOptions _options;
        private readonly IDistributedCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public JwtBuilder(IOptions<JwtOptions> options, IDistributedCache cache,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _options = options.Value;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;

        }
        public Action<AuthenticationOptions> AuthenticationOptions()
        {
            return options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            };
        }

        public async Task<bool> IsCurrentActiveToken()
        => await IsActiveAsync(GetCurrentAsync());

        public async Task DeactivateCurrentAsync()
            => await DeactivateAsync(GetCurrentAsync());

        public async Task<bool> IsActiveAsync(string token)
            => await _cache.GetStringAsync(GetKey(token)) == null;

        public async Task DeactivateAsync(string token)
            => await _cache.SetStringAsync(GetKey(token),
                " ", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(_options.ExpiryMinutes)
                });

        private string GetCurrentAsync()
        {
            var authorizationHeader = _httpContextAccessor
                .HttpContext.Request.Headers["authorization"];

            return authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();
        }

        private static string GetKey(string token)
            => $"tokens:{token}:deactivated";
        public string GetToken(Guid userId)
        {
            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
            var signinCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);
            var claims = new Claim[]
            {
                new Claim("userId",userId.ToString()),
            };
            var expirationDate = DateTime.Now.AddMinutes(_options.ExpiryMinutes);
            var jwt = new JwtSecurityToken(claims: claims, signingCredentials: signinCredentials, expires: expirationDate);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
        public string GetToken(Guid userId,string role)
        {
            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
            var signinCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);
            var claims = new Claim[]
            {
                new Claim("userId",userId.ToString()),
                new Claim("role",role.ToString()),
            };
            var expirationDate = DateTime.Now.AddMinutes(_options.ExpiryMinutes);
            var jwt = new JwtSecurityToken(claims: claims, signingCredentials: signinCredentials, expires: expirationDate);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        public Guid ValidateToken(string token)
        {
            var principal = GetPrincipal(token);
            if (principal == null)
            {
                return Guid.Empty;
            }

            ClaimsIdentity identity;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return Guid.Empty;
            }
            var userIdClaim = identity.FindFirst("userId");
            var userId = new Guid(userIdClaim.Value);
            return userId;
        }
        private ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                {
                    return null;
                }
                var key = Encoding.UTF8.GetBytes(_options.Secret);
                var parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                IdentityModelEventSource.ShowPII = true;
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
