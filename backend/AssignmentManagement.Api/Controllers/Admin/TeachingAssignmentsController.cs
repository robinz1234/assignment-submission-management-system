using AssignmentManagement.Api.Data;
using AssignmentManagement.Api.DTOs;
using AssignmentManagement.Api.Middleware;
using AssignmentManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Api.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/teaching-assignments")]
public class TeachingAssignmentsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TeachingAssignmentDto>>> GetAll()
    {
        var items = await dbContext.TeachingAssignments
            .Include(item => item.Teacher)
            .Include(item => item.Class)
            .Include(item => item.Subject)
            .AsNoTracking()
            .OrderBy(item => item.Teacher.FullName)
            .ThenBy(item => item.Class.Name)
            .ToListAsync();
        return Ok(items.Select(item => item.ToDto()));
    }

    [HttpPost]
    public async Task<ActionResult<TeachingAssignmentDto>> Create(CreateTeachingAssignmentRequest request)
    {
        var teacher = await dbContext.Users.SingleOrDefaultAsync(item => item.Id == request.TeacherId && item.IsActive)
            ?? throw new ApiException(StatusCodes.Status400BadRequest, "Teacher was not found.");
        if (teacher.Role != UserRole.Teacher)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "The selected user is not a teacher.");
        }

        var schoolClass = await dbContext.Classes.FindAsync(request.ClassId)
            ?? throw new ApiException(StatusCodes.Status400BadRequest, "Class was not found.");
        var subject = await dbContext.Subjects.FindAsync(request.SubjectId)
            ?? throw new ApiException(StatusCodes.Status400BadRequest, "Subject was not found.");

        if (await dbContext.TeachingAssignments.AnyAsync(item =>
                item.TeacherId == request.TeacherId && item.ClassId == request.ClassId && item.SubjectId == request.SubjectId))
        {
            throw new ApiException(StatusCodes.Status409Conflict, "This teaching assignment already exists.");
        }

        var entity = new TeachingAssignment
        {
            Teacher = teacher,
            Class = schoolClass,
            Subject = subject
        };
        dbContext.TeachingAssignments.Add(entity);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, entity.ToDto());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await dbContext.TeachingAssignments.SingleOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Teaching assignment was not found.");
        var hasAssignments = await dbContext.Assignments.AnyAsync(item =>
            item.TeacherId == entity.TeacherId && item.ClassId == entity.ClassId && item.SubjectId == entity.SubjectId);
        if (hasAssignments)
        {
            throw new ApiException(StatusCodes.Status409Conflict, "This teaching assignment is used by existing assignments.");
        }

        dbContext.TeachingAssignments.Remove(entity);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
