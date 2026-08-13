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
[Route("api/admin/subjects")]
public class SubjectsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SubjectDto>>> GetAll()
    {
        var subjects = await dbContext.Subjects.AsNoTracking().OrderBy(item => item.Name).ToListAsync();
        return Ok(subjects.Select(item => item.ToDto()));
    }

    [HttpPost]
    public async Task<ActionResult<SubjectDto>> Create(UpsertSubjectRequest request)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await dbContext.Subjects.AnyAsync(item => item.Code == code))
        {
            throw new ApiException(StatusCodes.Status409Conflict, "A subject with this code already exists.");
        }

        var entity = new Subject { Name = request.Name.Trim(), Code = code };
        dbContext.Subjects.Add(entity);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, entity.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SubjectDto>> Update(Guid id, UpsertSubjectRequest request)
    {
        var entity = await dbContext.Subjects.SingleOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Subject was not found.");
        var code = request.Code.Trim().ToUpperInvariant();
        if (await dbContext.Subjects.AnyAsync(item => item.Code == code && item.Id != id))
        {
            throw new ApiException(StatusCodes.Status409Conflict, "A subject with this code already exists.");
        }

        entity.Name = request.Name.Trim();
        entity.Code = code;
        await dbContext.SaveChangesAsync();
        return Ok(entity.ToDto());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await dbContext.Subjects.SingleOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Subject was not found.");
        var inUse = await dbContext.Assignments.AnyAsync(item => item.SubjectId == id)
            || await dbContext.TeachingAssignments.AnyAsync(item => item.SubjectId == id);
        if (inUse)
        {
            throw new ApiException(StatusCodes.Status409Conflict, "This subject is in use and cannot be deleted.");
        }

        dbContext.Subjects.Remove(entity);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
