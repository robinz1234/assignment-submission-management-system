using System.Security.Claims;
using AssignmentManagement.Api.Models;

namespace AssignmentManagement.Api.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal User => httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException("No authenticated request is available.");

    public Guid UserId
    {
        get
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id)
                ? id
                : throw new UnauthorizedAccessException("The access token does not contain a valid user identifier.");
        }
    }

    public UserRole Role
    {
        get
        {
            var value = User.FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<UserRole>(value, true, out var role)
                ? role
                : throw new UnauthorizedAccessException("The access token does not contain a valid role.");
        }
    }

    public Guid? ClassId
    {
        get
        {
            var value = User.FindFirstValue("classId");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }
}
