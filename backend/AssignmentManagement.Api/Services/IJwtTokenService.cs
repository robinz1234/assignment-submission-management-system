using AssignmentManagement.Api.Models;

namespace AssignmentManagement.Api.Services;

public record TokenResult(string Token, DateTimeOffset ExpiresAt);

public interface IJwtTokenService
{
    TokenResult CreateToken(AppUser user);
}
