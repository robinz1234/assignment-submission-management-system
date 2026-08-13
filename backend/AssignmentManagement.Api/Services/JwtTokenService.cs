using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AssignmentManagement.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace AssignmentManagement.Api.Services;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public TokenResult CreateToken(AppUser user)
    {
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var expiryMinutes = configuration.GetValue<int?>("Jwt:ExpiryMinutes") ?? 480;
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.ClassId.HasValue)
        {
            claims.Add(new Claim("classId", user.ClassId.Value.ToString()));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new TokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
