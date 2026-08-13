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
[Route("api/submissions")]
public class SubmissionsController(
    AppDbContext dbContext,
    ICurrentUserService currentUser,
    ISubmissionWorkflowService workflowService) : ControllerBase
{
    [Authorize(Roles = "Teacher,Admin")]
    [HttpGet]
    public async Task<ActionResult<PagedResult<SubmissionDto>>> GetAll(
        [FromQuery] SubmissionStatus? status,
        [FromQuery] Guid? assignmentId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = dbContext.Submissions
            .Include(item => item.Assignment)
            .Include(item => item.Student)
            .AsNoTracking()
            .AsQueryable();

        if (currentUser.Role == UserRole.Teacher)
        {
            query = query.Where(item => item.Assignment.TeacherId == currentUser.UserId);
        }

        if (status.HasValue)
        {
            query = query.Where(item => item.Status == status.Value);
        }

        if (assignmentId.HasValue)
        {
            query = query.Where(item => item.AssignmentId == assignmentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";
            query = query.Where(item =>
                EF.Functions.ILike(item.Assignment.Title, term) ||
                EF.Functions.ILike(item.Student.FullName, term));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(item => item.SubmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new PagedResult<SubmissionDto>(
            items.Select(item => item.ToDto()).ToList(),
            page,
            pageSize,
            totalCount));
    }

    [Authorize(Roles = "Student")]
    [HttpGet("my")]
    public async Task<ActionResult<IReadOnlyList<SubmissionDto>>> MySubmissions()
    {
        var items = await dbContext.Submissions
            .Where(item => item.StudentId == currentUser.UserId)
            .Include(item => item.Assignment)
            .Include(item => item.Student)
            .AsNoTracking()
            .OrderByDescending(item => item.SubmittedAt)
            .ToListAsync();
        return Ok(items.Select(item => item.ToDto()));
    }

    [Authorize(Roles = "Student")]
    [HttpPost("assignments/{assignmentId:guid}")]
    public async Task<ActionResult<SubmissionDto>> Submit(Guid assignmentId, SubmitAnswerRequest request)
    {
        var assignment = await dbContext.Assignments.SingleOrDefaultAsync(item => item.Id == assignmentId)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Assignment was not found.");
        var student = await dbContext.Users.SingleOrDefaultAsync(item => item.Id == currentUser.UserId && item.IsActive)
            ?? throw new ApiException(StatusCodes.Status401Unauthorized, "Student account was not found.");
        var existing = await dbContext.Submissions.SingleOrDefaultAsync(item =>
            item.AssignmentId == assignmentId && item.StudentId == currentUser.UserId);
        workflowService.EnsureStudentCanCreate(assignment, student, existing, DateTimeOffset.UtcNow);

        var submission = new Submission
        {
            Assignment = assignment,
            Student = student,
            AnswerText = request.AnswerText.Trim(),
            Status = SubmissionStatus.Submitted
        };
        dbContext.Submissions.Add(submission);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = submission.Id }, submission.ToDto());
    }

    [Authorize(Roles = "Student")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SubmissionDto>> Update(Guid id, SubmitAnswerRequest request)
    {
        var submission = await dbContext.Submissions
            .Include(item => item.Assignment)
            .Include(item => item.Student)
            .SingleOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Submission was not found.");
        workflowService.EnsureStudentCanUpdate(submission.Assignment, submission.Student, submission, DateTimeOffset.UtcNow);
        submission.AnswerText = request.AnswerText.Trim();
        submission.UpdatedAt = DateTimeOffset.UtcNow;
        submission.Status = SubmissionStatus.Submitted;
        submission.Marks = null;
        submission.Feedback = null;
        submission.ReviewedAt = null;
        await dbContext.SaveChangesAsync();
        return Ok(submission.ToDto());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SubmissionDto>> GetById(Guid id)
    {
        var submission = await dbContext.Submissions
            .Include(item => item.Assignment)
            .Include(item => item.Student)
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Submission was not found.");

        var allowed = currentUser.Role switch
        {
            UserRole.Admin => true,
            UserRole.Teacher => submission.Assignment.TeacherId == currentUser.UserId,
            UserRole.Student => submission.StudentId == currentUser.UserId,
            _ => false
        };
        if (!allowed)
        {
            throw new ApiException(StatusCodes.Status403Forbidden, "You cannot view this submission.");
        }
        return Ok(submission.ToDto());
    }

    [Authorize(Roles = "Teacher")]
    [HttpPut("{id:guid}/review")]
    public async Task<ActionResult<SubmissionDto>> Review(Guid id, ReviewSubmissionRequest request)
    {
        var submission = await dbContext.Submissions
            .Include(item => item.Assignment)
            .Include(item => item.Student)
            .SingleOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Submission was not found.");
        workflowService.EnsureTeacherCanReview(submission.Assignment, submission, currentUser.UserId, request.Marks);
        if (request.Status == SubmissionStatus.Submitted)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Use Reviewed or Returned as the review status.");
        }

        submission.Marks = request.Marks;
        submission.Feedback = string.IsNullOrWhiteSpace(request.Feedback) ? null : request.Feedback.Trim();
        submission.Status = request.Status;
        submission.ReviewedAt = DateTimeOffset.UtcNow;
        submission.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync();
        return Ok(submission.ToDto());
    }
}
