using AssignmentManagement.Api.Data;
using AssignmentManagement.Api.DTOs;
using AssignmentManagement.Api.Middleware;
using AssignmentManagement.Api.Models;
using AssignmentManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Api.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/users")]
public class UsersController(
    AppDbContext dbContext,
    IPasswordHasher passwordHasher,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] UserRole? role,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = dbContext.Users.Include(item => item.Class).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";
            query = query.Where(item =>
                EF.Functions.ILike(item.FullName, term) || EF.Functions.ILike(item.Email, term));
        }

        if (role.HasValue)
        {
            query = query.Where(item => item.Role == role.Value);
        }

        var totalCount = await query.CountAsync();
        var users = await query
            .OrderBy(item => item.Role)
            .ThenBy(item => item.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new PagedResult<UserDto>(users.Select(item => item.ToDto()).ToList(), page, pageSize, totalCount));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await dbContext.Users.Include(item => item.Class).AsNoTracking().SingleOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "User was not found.");
        return Ok(user.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await dbContext.Users.AnyAsync(item => item.Email == email))
        {
            throw new ApiException(StatusCodes.Status409Conflict, "A user with this email already exists.");
        }

        await ValidateClassForRoleAsync(request.Role, request.ClassId);
        var user = new AppUser
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = request.Role,
            ClassId = request.Role == UserRole.Student ? request.ClassId : null
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(user).Reference(item => item.Class).LoadAsync();
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, UpdateUserRequest request)
    {
        var user = await dbContext.Users.Include(item => item.Class).SingleOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "User was not found.");

        var email = request.Email.Trim().ToLowerInvariant();
        if (await dbContext.Users.AnyAsync(item => item.Email == email && item.Id != id))
        {
            throw new ApiException(StatusCodes.Status409Conflict, "A user with this email already exists.");
        }

        if (id == currentUser.UserId && !request.IsActive)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "You cannot deactivate your own account.");
        }

        await ValidateClassForRoleAsync(request.Role, request.ClassId);
        user.FullName = request.FullName.Trim();
        user.Email = email;
        user.Role = request.Role;
        user.ClassId = request.Role == UserRole.Student ? request.ClassId : null;
        user.IsActive = request.IsActive;
        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        }

        await dbContext.SaveChangesAsync();
        await dbContext.Entry(user).Reference(item => item.Class).LoadAsync();
        return Ok(user.ToDto());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "User was not found.");

        if (id == currentUser.UserId)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "You cannot deactivate your own account.");
        }

        user.IsActive = false;
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    private async Task ValidateClassForRoleAsync(UserRole role, Guid? classId)
    {
        if (role == UserRole.Student && !classId.HasValue)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "A class is required for a student account.");
        }

        if (role == UserRole.Student && !await dbContext.Classes.AnyAsync(item => item.Id == classId))
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "The selected class does not exist.");
        }
    }
}
