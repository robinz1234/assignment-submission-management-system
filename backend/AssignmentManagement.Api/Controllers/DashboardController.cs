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
[Route("api/dashboard")]
public class DashboardController(AppDbContext dbContext, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get()
    {
        if (currentUser.Role == UserRole.Admin)
        {
            return Ok(await BuildAdminAsync());
        }

        if (currentUser.Role == UserRole.Teacher)
        {
            return Ok(await BuildTeacherAsync());
        }

        if (currentUser.Role == UserRole.Student)
        {
            return Ok(await BuildStudentAsync());
        }

        return Forbid();
    }

    private async Task<DashboardDto> BuildAdminAsync()
    {
        var recentAssignments = await dbContext.Assignments
            .Include(item => item.Teacher).Include(item => item.Class).Include(item => item.Subject).Include(item => item.Submissions)
            .AsNoTracking().OrderByDescending(item => item.CreatedAt).Take(5).ToListAsync();
        var recentSubmissions = await dbContext.Submissions
            .Include(item => item.Assignment).Include(item => item.Student)
            .AsNoTracking().OrderByDescending(item => item.SubmittedAt).Take(5).ToListAsync();
        return new DashboardDto("Admin",
        [
            new("Active users", await dbContext.Users.CountAsync(item => item.IsActive), "Across all roles"),
            new("Classes", await dbContext.Classes.CountAsync(), "Academic groups"),
            new("Assignments", await dbContext.Assignments.CountAsync(), "Draft and published"),
            new("Submissions", await dbContext.Submissions.CountAsync(), "All student work")
        ], recentAssignments.Select(item => item.ToDto()).ToList(), recentSubmissions.Select(item => item.ToDto()).ToList());
    }

    private async Task<DashboardDto> BuildTeacherAsync()
    {
        var assignmentsQuery = dbContext.Assignments.Where(item => item.TeacherId == currentUser.UserId);
        var recentAssignments = await assignmentsQuery
            .Include(item => item.Teacher).Include(item => item.Class).Include(item => item.Subject).Include(item => item.Submissions)
            .AsNoTracking().OrderByDescending(item => item.CreatedAt).Take(5).ToListAsync();
        var recentSubmissions = await dbContext.Submissions
            .Where(item => item.Assignment.TeacherId == currentUser.UserId)
            .Include(item => item.Assignment).Include(item => item.Student)
            .AsNoTracking().OrderByDescending(item => item.SubmittedAt).Take(5).ToListAsync();
        return new DashboardDto("Teacher",
        [
            new("My assignments", await assignmentsQuery.CountAsync(), "All created assignments"),
            new("Published", await assignmentsQuery.CountAsync(item => item.Status == AssignmentStatus.Published), "Visible to students"),
            new("Awaiting review", await dbContext.Submissions.CountAsync(item => item.Assignment.TeacherId == currentUser.UserId && item.Status == SubmissionStatus.Submitted), "Submitted work"),
            new("Reviewed", await dbContext.Submissions.CountAsync(item => item.Assignment.TeacherId == currentUser.UserId && item.Status == SubmissionStatus.Reviewed), "Completed reviews")
        ], recentAssignments.Select(item => item.ToDto()).ToList(), recentSubmissions.Select(item => item.ToDto()).ToList());
    }

    private async Task<DashboardDto> BuildStudentAsync()
    {
        var classId = currentUser.ClassId;
        var assignmentsQuery = dbContext.Assignments.Where(item =>
            classId.HasValue && item.ClassId == classId.Value && item.Status == AssignmentStatus.Published);
        var recentAssignments = await assignmentsQuery
            .Include(item => item.Teacher).Include(item => item.Class).Include(item => item.Subject).Include(item => item.Submissions)
            .AsNoTracking().OrderBy(item => item.Deadline).Take(5).ToListAsync();
        var recentSubmissions = await dbContext.Submissions
            .Where(item => item.StudentId == currentUser.UserId)
            .Include(item => item.Assignment).Include(item => item.Student)
            .AsNoTracking().OrderByDescending(item => item.SubmittedAt).Take(5).ToListAsync();
        return new DashboardDto("Student",
        [
            new("Available", await assignmentsQuery.CountAsync(), "Published assignments"),
            new("Due soon", await assignmentsQuery.CountAsync(item => item.Deadline <= DateTimeOffset.UtcNow.AddDays(3) && item.Deadline > DateTimeOffset.UtcNow), "Within three days"),
            new("Submitted", await dbContext.Submissions.CountAsync(item => item.StudentId == currentUser.UserId), "Your submissions"),
            new("Reviewed", await dbContext.Submissions.CountAsync(item => item.StudentId == currentUser.UserId && item.Status == SubmissionStatus.Reviewed), "Marked by teachers")
        ], recentAssignments.Select(item => item.ToDto(currentUser.UserId)).ToList(), recentSubmissions.Select(item => item.ToDto()).ToList());
    }
}
