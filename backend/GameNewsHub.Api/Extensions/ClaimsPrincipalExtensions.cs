using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GameNewsHub.Api.Constants;
using GameNewsHub.Data.Constants;

namespace GameNewsHub.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return id != null ? int.Parse(id) : throw new UnauthorizedAccessException();
    }

    public static bool isAdmin(this ClaimsPrincipal user)
    {
        var val = user.IsInRole(Roles.Admin);
        return val;
    }
}