using System.ComponentModel.DataAnnotations;
using AssignmentManagement.Api.Models;

namespace AssignmentManagement.Api.DTOs;

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public record CurrentUserDto(
    Guid Id,
    string FullName,
    string Email,
    UserRole Role,
    Guid? ClassId,
    string? ClassName);

public record AuthResponse(string Token, DateTimeOffset ExpiresAt, CurrentUserDto User);
