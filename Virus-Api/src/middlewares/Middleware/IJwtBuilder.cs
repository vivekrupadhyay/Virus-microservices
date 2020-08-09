using System;
using System.Threading.Tasks;

namespace Middleware
{
    public interface IJwtBuilder
    {
        string GetToken(Guid userId);
        Guid ValidateToken(string token);
        Task<bool> IsCurrentActiveToken();
        Task DeactivateCurrentAsync();
        Task<bool> IsActiveAsync(string token);
        Task DeactivateAsync(string token);
    }
}
