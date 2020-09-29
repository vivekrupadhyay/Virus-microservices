using System;

namespace Middleware
{
    public class JwtOptions
    {
        public string Secret { get; set; }
        public int ExpiryMinutes { get; set; }

        public string TOKEN_ISSUER { get; set; }
    }
}
