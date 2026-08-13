using AssignmentManagement.Api.Data;
using AssignmentManagement.Api.DTOs;
using AssignmentManagement.Api.Models;
using AssignmentManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/reference")]
public class ReferenceController(AppDbContext dbContext, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet("classes")]
    public async Task<ActionResult<IReadOnlyList<OptionDto>>> Classes()
    {
        var items = await dbContext.Classes.AsNoTracking()
            .OrderBy(item => item.Name).ThenBy(item => item.Section)
            .Select(item => new OptionDto(item.Id, item.Name + " - " + item.Section + " (" + item.AcademicYear + ")"))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("subjects")]
    public async Task<ActionResult<IReadOnlyList<OptionDto>>> Subjects()
    {
        var query = dbContext.Subjects.AsNoTracking().AsQueryable();
        if (currentUser.Role == UserRole.Teacher)
        {
            query = query.Where(subject => dbContext.TeachingAssignments.Any(item =>
                item.TeacherId == currentUser.UserId && item.SubjectId == subject.Id));
        }

        var items = await query.OrderBy(item => item.Name)
            .Select(item => new OptionDto(item.Id, item.Name + " (" + item.Code + ")"))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("teacher-scopes")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<IReadOnlyList<TeachingAssignmentDto>>> TeacherScopes()
    {
        var items = await dbContext.TeachingAssignments
            .Where(item => item.TeacherId == currentUser.UserId)
            .Include(item => item.Teacher)
            .Include(item => item.Class)
            .Include(item => item.Subject)
            .AsNoTracking()
            .OrderBy(item => item.Class.Name)
            .ThenBy(item => item.Subject.Name)
            .ToListAsync();
        return Ok(items.Select(item => item.ToDto()));
    }
}
