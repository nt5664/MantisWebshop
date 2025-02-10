using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MantisWebshop.Server.Extensions
{
    public static class UserExtensions
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
