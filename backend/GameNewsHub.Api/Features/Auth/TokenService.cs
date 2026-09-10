using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Configuration;
using GameNewsHub.Api.Constants;
using GameNewsHub.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GameNewsHub.Api.Features.Auth;

public class TokenService
{
    private readonly JwtTokenOptions _jwtTokenOptions;
    private readonly UserManager<AppUser> _userManager;

    public TokenService(UserManager<AppUser> userManager, IOptions<JwtTokenOptions> jwtOptions)
    {
        _jwtTokenOptions = jwtOptions.Value;
        _userManager = userManager;
    }
    
    public async Task<string> GenerateAccessToken(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("token_type", "access"),
        };
        //jwt registered claim names doesn't have roles i think
        // and default claim names fron .net are toooo long
        claims.AddRange(roles.Select(role => new Claim("roles", role)));
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenOptions.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // jak admin to ttl mniejszy
        var expires = roles.Contains(Roles.Admin)
            ? DateTime.UtcNow.Add(_jwtTokenOptions.AdminAccessTokenLifetime)
            : DateTime.UtcNow.Add(_jwtTokenOptions.AccessTokenLifetime);
        
        var token = new JwtSecurityToken(
            issuer: _jwtTokenOptions.Issuer,
            audience: _jwtTokenOptions.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("token_type", "refresh"),
            new Claim("security_stamp", user.SecurityStamp)
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenOptions.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtTokenOptions.Issuer,
            audience: _jwtTokenOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(_jwtTokenOptions.RefreshTokenLifetime),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateRefreshToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtTokenOptions.Issuer,
            ValidAudience = _jwtTokenOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenOptions.SigningKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        try
        {
            var principal = handler.ValidateToken(token, validationParams, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var tokenType = principal.FindFirstValue("token_type");
            return tokenType == "refresh" ? principal : null;
        }
        catch
        {
            return null;
        }
    }
    
    
}