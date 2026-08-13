using AssignmentManagement.Api.Data;
using AssignmentManagement.Api.DTOs;
using AssignmentManagement.Api.Middleware;
using AssignmentManagement.Api.Models;
using AssignmentManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    AppDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtTokenService tokenService,
    ICurrentUserService currentUser) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await dbContext.Users
            .Include(item => item.Class)
            .SingleOrDefaultAsync(item => item.Email == normalizedEmail);

        if (user is null || !user.IsActive || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new ApiException(StatusCodes.Status401Unauthorized, "Email or password is incorrect.");
        }

        var token = tokenService.CreateToken(user);
        return Ok(new AuthResponse(token.Token, token.ExpiresAt, ToCurrentUser(user)));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserDto>> Me()
    {
        var user = await dbContext.Users
            .Include(item => item.Class)
            .SingleOrDefaultAsync(item => item.Id == currentUser.UserId && item.IsActive)
            ?? throw new ApiException(StatusCodes.Status401Unauthorized, "The user account is not available.");

        return Ok(ToCurrentUser(user));
    }

    private static CurrentUserDto ToCurrentUser(AppUser user) => new(
        user.Id,
        user.FullName,
        user.Email,
        user.Role,
        user.ClassId,
        user.Class is null ? null : $"{user.Class.Name} - {user.Class.Section}");
}
