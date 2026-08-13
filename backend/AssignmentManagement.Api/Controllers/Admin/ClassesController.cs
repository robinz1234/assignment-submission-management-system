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
[Route("api/admin/classes")]
public class ClassesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClassDto>>> GetAll()
    {
        var classes = await dbContext.Classes
            .Include(item => item.Students)
            .AsNoTracking()
            .OrderByDescending(item => item.AcademicYear)
            .ThenBy(item => item.Name)
            .ThenBy(item => item.Section)
            .ToListAsync();
        return Ok(classes.Select(item => item.ToDto()));
    }

    [HttpPost]
    public async Task<ActionResult<ClassDto>> Create(UpsertClassRequest request)
    {
        var entity = new SchoolClass
        {
            Name = request.Name.Trim(),
            Section = request.Section.Trim(),
            AcademicYear = request.AcademicYear.Trim()
        };
        dbContext.Classes.Add(entity);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, entity.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClassDto>> Update(Guid id, UpsertClassRequest request)
    {
        var entity = await dbContext.Classes.Include(item => item.Students).SingleOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Class was not found.");
        entity.Name = request.Name.Trim();
        entity.Section = request.Section.Trim();
        entity.AcademicYear = request.AcademicYear.Trim();
        await dbContext.SaveChangesAsync();
        return Ok(entity.ToDto());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await dbContext.Classes.SingleOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Class was not found.");
        var inUse = await dbContext.Users.AnyAsync(item => item.ClassId == id)
            || await dbContext.Assignments.AnyAsync(item => item.ClassId == id)
            || await dbContext.TeachingAssignments.AnyAsync(item => item.ClassId == id);
        if (inUse)
        {
            throw new ApiException(StatusCodes.Status409Conflict, "This class is in use and cannot be deleted.");
        }

        dbContext.Classes.Remove(entity);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
