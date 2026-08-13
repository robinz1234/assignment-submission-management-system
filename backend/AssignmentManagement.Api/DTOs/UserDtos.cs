using System.ComponentModel.DataAnnotations;
using AssignmentManagement.Api.Models;

namespace AssignmentManagement.Api.DTOs;

public record UserDto(
    Guid Id,
    string FullName,
    string Email,
    UserRole Role,
    Guid? ClassId,
    string? ClassName,
    bool IsActive,
    DateTimeOffset CreatedAt);

public class CreateUserRequest
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(180)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public Guid? ClassId { get; set; }
}

public class UpdateUserRequest
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(180)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public Guid? ClassId { get; set; }
    public bool IsActive { get; set; } = true;

    [MinLength(8), MaxLength(100)]
    public string? NewPassword { get; set; }
}
