using AssignmentManagement.Api.Middleware;
using AssignmentManagement.Api.Models;
using AssignmentManagement.Api.Services;
using Microsoft.AspNetCore.Http;

namespace AssignmentManagement.Tests;

public class SubmissionWorkflowServiceTests
{
    private readonly SubmissionWorkflowService _service = new();

    [Fact]
    public void StudentCannotSubmitAfterDeadline()
    {
        var student = Student();
        var assignment = AssignmentFor(student.ClassId!.Value, DateTimeOffset.UtcNow.AddMinutes(-1));

        var exception = Assert.Throws<ApiException>(() =>
            _service.EnsureStudentCanCreate(assignment, student, null, DateTimeOffset.UtcNow));

        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        Assert.Contains("deadline", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StudentCannotSubmitToAnotherClass()
    {
        var student = Student();
        var assignment = AssignmentFor(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(1));

        var exception = Assert.Throws<ApiException>(() =>
            _service.EnsureStudentCanCreate(assignment, student, null, DateTimeOffset.UtcNow));

        Assert.Equal(StatusCodes.Status403Forbidden, exception.StatusCode);
    }

    [Fact]
    public void StudentCannotUpdateWhenResubmissionIsDisabled()
    {
        var student = Student();
        var assignment = AssignmentFor(student.ClassId!.Value, DateTimeOffset.UtcNow.AddDays(1));
        assignment.AllowResubmission = false;
        var submission = new Submission
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Status = SubmissionStatus.Submitted
        };

        var exception = Assert.Throws<ApiException>(() =>
            _service.EnsureStudentCanUpdate(assignment, student, submission, DateTimeOffset.UtcNow));

        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        Assert.Contains("does not allow", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TeacherCannotGiveMarksAboveMaximum()
    {
        var teacherId = Guid.NewGuid();
        var assignment = AssignmentFor(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(1));
        assignment.TeacherId = teacherId;
        assignment.MaxMarks = 20;
        var submission = new Submission { AssignmentId = assignment.Id };

        var exception = Assert.Throws<ApiException>(() =>
            _service.EnsureTeacherCanReview(assignment, submission, teacherId, 21));

        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
        Assert.Contains("between 0 and 20", exception.Message);
    }

    [Fact]
    public void AssignedTeacherCanReviewValidMarks()
    {
        var teacherId = Guid.NewGuid();
        var assignment = AssignmentFor(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(1));
        assignment.TeacherId = teacherId;
        assignment.MaxMarks = 20;
        var submission = new Submission { AssignmentId = assignment.Id };

        var exception = Record.Exception(() =>
            _service.EnsureTeacherCanReview(assignment, submission, teacherId, 18));

        Assert.Null(exception);
    }

    private static AppUser Student()
    {
        return new AppUser
        {
            Id = Guid.NewGuid(),
            Role = UserRole.Student,
            ClassId = Guid.NewGuid()
        };
    }

    private static Assignment AssignmentFor(Guid classId, DateTimeOffset deadline)
    {
        return new Assignment
        {
            Id = Guid.NewGuid(),
            ClassId = classId,
            Deadline = deadline,
            Status = AssignmentStatus.Published,
            AllowResubmission = true,
            MaxMarks = 20
        };
    }
}
