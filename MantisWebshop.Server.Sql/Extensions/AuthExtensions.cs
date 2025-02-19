using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace MantisWebshop.Server.Sql.Extensions
{
    public static class AuthExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal principal)
        {
            try
            {
                return principal.Claims.Single(x => x.Type.Equals(JwtRegisteredClaimNames.Name)).Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
