using AssignmentManagement.Api.Models;

namespace AssignmentManagement.Api.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
    UserRole Role { get; }
    Guid? ClassId { get; }
}
