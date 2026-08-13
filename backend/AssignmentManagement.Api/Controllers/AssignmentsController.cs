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
[Authorize]
[Route("api/assignments")]
public class AssignmentsController(AppDbContext dbContext, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<AssignmentDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] AssignmentStatus? status,
        [FromQuery] Guid? classId,
        [FromQuery] Guid? subjectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var query = AccessibleAssignments()
            .Include(item => item.Teacher)
            .Include(item => item.Class)
            .Include(item => item.Subject)
            .Include(item => item.Submissions)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";
            query = query.Where(item =>
                EF.Functions.ILike(item.Title, term) ||
                EF.Functions.ILike(item.Description, term) ||
                EF.Functions.ILike(item.Subject.Name, term));
        }
        if (status.HasValue) query = query.Where(item => item.Status == status.Value);
        if (classId.HasValue) query = query.Where(item => item.ClassId == classId.Value);
        if (subjectId.HasValue) query = query.Where(item => item.SubjectId == subjectId.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(item => item.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var studentId = currentUser.Role == UserRole.Student ? currentUser.UserId : (Guid?)null;
        return Ok(new PagedResult<AssignmentDto>(items.Select(item => item.ToDto(studentId)).ToList(), page, pageSize, totalCount));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AssignmentDto>> GetById(Guid id)
    {
        var item = await AccessibleAssignments()
            .Include(entity => entity.Teacher)
            .Include(entity => entity.Class)
            .Include(entity => entity.Subject)
            .Include(entity => entity.Submissions)
            .AsNoTracking()
            .SingleOrDefaultAsync(entity => entity.Id == id)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Assignment was not found.");

        var studentId = currentUser.Role == UserRole.Student ? currentUser.UserId : (Guid?)null;
        return Ok(item.ToDto(studentId));
    }

    [Authorize(Roles = "Teacher")]
    [HttpPost]
    public async Task<ActionResult<AssignmentDto>> Create(CreateAssignmentRequest request)
    {
        ValidateAssignmentDates(request.Deadline);
        await EnsureTeachingScopeAsync(request.ClassId, request.SubjectId);

        var item = new Assignment
        {
            TeacherId = currentUser.UserId,
            ClassId = request.ClassId,
            SubjectId = request.SubjectId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Deadline = request.Deadline.ToUniversalTime(),
            MaxMarks = request.MaxMarks,
            Status = request.Status,
            AllowResubmission = request.AllowResubmission
        };

        dbContext.Assignments.Add(item);
        await dbContext.SaveChangesAsync();
        await LoadAssignmentReferencesAsync(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item.ToDto());
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AssignmentDto>> Update(Guid id, UpdateAssignmentRequest request)
    {
        var item = await dbContext.Assignments
            .Include(entity => entity.Submissions)
            .SingleOrDefaultAsync(entity => entity.Id == id && entity.TeacherId == currentUser.UserId)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Assignment was not found.");

        ValidateAssignmentDates(request.Deadline);
        await EnsureTeachingScopeAsync(request.ClassId, request.SubjectId);
        if (item.Submissions.Count > 0 && (item.ClassId != request.ClassId || item.SubjectId != request.SubjectId))
        {
            throw new ApiException(StatusCodes.Status409Conflict, "The class or subject cannot be changed after submissions exist.");
        }

        item.ClassId = request.ClassId;
        item.SubjectId = request.SubjectId;
        item.Title = request.Title.Trim();
        item.Description = request.Description.Trim();
        item.Deadline = request.Deadline.ToUniversalTime();
        item.MaxMarks = request.MaxMarks;
        item.Status = request.Status;
        item.AllowResubmission = request.AllowResubmission;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync();
        await LoadAssignmentReferencesAsync(item);
        return Ok(item.ToDto());
    }

    [Authorize(Roles = "Teacher")]
    [HttpPost("{id:guid}/publish")]
    public async Task<ActionResult<AssignmentDto>> Publish(Guid id)
    {
        var item = await dbContext.Assignments
            .SingleOrDefaultAsync(entity => entity.Id == id && entity.TeacherId == currentUser.UserId)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Assignment was not found.");
        ValidateAssignmentDates(item.Deadline);
        item.Status = AssignmentStatus.Published;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync();
        await LoadAssignmentReferencesAsync(item);
        return Ok(item.ToDto());
    }

    [Authorize(Roles = "Teacher")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var item = await dbContext.Assignments
            .Include(entity => entity.Submissions)
            .SingleOrDefaultAsync(entity => entity.Id == id && entity.TeacherId == currentUser.UserId)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Assignment was not found.");
        if (item.Submissions.Count > 0)
        {
            throw new ApiException(StatusCodes.Status409Conflict, "An assignment with submissions cannot be deleted.");
        }

        dbContext.Assignments.Remove(item);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Teacher,Admin")]
    [HttpGet("{id:guid}/submissions")]
    public async Task<ActionResult<PagedResult<SubmissionDto>>> GetSubmissions(
        Guid id,
        [FromQuery] SubmissionStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var assignment = await dbContext.Assignments.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Assignment was not found.");
        if (currentUser.Role == UserRole.Teacher && assignment.TeacherId != currentUser.UserId)
        {
            throw new ApiException(StatusCodes.Status403Forbidden, "You cannot view submissions for another teacher's assignment.");
        }

        var query = dbContext.Submissions
            .Where(item => item.AssignmentId == id)
            .Include(item => item.Assignment)
            .Include(item => item.Student)
            .AsNoTracking();
        if (status.HasValue) query = query.Where(item => item.Status == status.Value);
        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(item => item.SubmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Ok(new PagedResult<SubmissionDto>(items.Select(item => item.ToDto()).ToList(), page, pageSize, totalCount));
    }

    private IQueryable<Assignment> AccessibleAssignments()
    {
        var query = dbContext.Assignments.AsQueryable();
        return currentUser.Role switch
        {
            UserRole.Admin => query,
            UserRole.Teacher => query.Where(item => item.TeacherId == currentUser.UserId),
            UserRole.Student when currentUser.ClassId.HasValue => query.Where(item =>
                item.ClassId == currentUser.ClassId.Value && item.Status == AssignmentStatus.Published),
            _ => query.Where(_ => false)
        };
    }

    private async Task EnsureTeachingScopeAsync(Guid classId, Guid subjectId)
    {
        var allowed = await dbContext.TeachingAssignments.AnyAsync(item =>
            item.TeacherId == currentUser.UserId && item.ClassId == classId && item.SubjectId == subjectId);
        if (!allowed)
        {
            throw new ApiException(StatusCodes.Status403Forbidden, "You are not assigned to teach this subject for the selected class.");
        }
    }

    private static void ValidateAssignmentDates(DateTimeOffset deadline)
    {
        if (deadline.ToUniversalTime() <= DateTimeOffset.UtcNow)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "The deadline must be in the future.");
        }
    }

    private async Task LoadAssignmentReferencesAsync(Assignment item)
    {
        await dbContext.Entry(item).Reference(entity => entity.Teacher).LoadAsync();
        await dbContext.Entry(item).Reference(entity => entity.Class).LoadAsync();
        await dbContext.Entry(item).Reference(entity => entity.Subject).LoadAsync();
        if (!dbContext.Entry(item).Collection(entity => entity.Submissions).IsLoaded)
        {
            await dbContext.Entry(item).Collection(entity => entity.Submissions).LoadAsync();
        }
    }
}
