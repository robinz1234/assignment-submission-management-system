using AssignmentManagement.Api.Middleware;
using AssignmentManagement.Api.Models;

namespace AssignmentManagement.Api.Services;

public class SubmissionWorkflowService : ISubmissionWorkflowService
{
    public void EnsureStudentCanCreate(
        Assignment assignment,
        AppUser student,
        Submission? existingSubmission,
        DateTimeOffset now)
    {
        EnsureStudentCanAccessAssignment(assignment, student, now);

        if (existingSubmission is not null)
        {
            throw new ApiException(StatusCodes.Status409Conflict, "A submission already exists. Use the update endpoint instead.");
        }
    }

    public void EnsureStudentCanUpdate(
        Assignment assignment,
        AppUser student,
        Submission submission,
        DateTimeOffset now)
    {
        EnsureStudentCanAccessAssignment(assignment, student, now);

        if (submission.StudentId != student.Id)
        {
            throw new ApiException(StatusCodes.Status403Forbidden, "You cannot update another student's submission.");
        }

        if (!assignment.AllowResubmission)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "This assignment does not allow submission updates.");
        }

        if (submission.Status == SubmissionStatus.Reviewed)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "A reviewed submission cannot be updated.");
        }
    }

    public void EnsureTeacherCanReview(
        Assignment assignment,
        Submission submission,
        Guid teacherId,
        decimal? marks)
    {
        if (assignment.TeacherId != teacherId)
        {
            throw new ApiException(StatusCodes.Status403Forbidden, "You can review submissions only for your own assignments.");
        }

        if (submission.AssignmentId != assignment.Id)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "The submission does not belong to this assignment.");
        }

        if (marks.HasValue && (marks.Value < 0 || marks.Value > assignment.MaxMarks))
        {
            throw new ApiException(
                StatusCodes.Status400BadRequest,
                $"Marks must be between 0 and {assignment.MaxMarks}.");
        }
    }

    private static void EnsureStudentCanAccessAssignment(
        Assignment assignment,
        AppUser student,
        DateTimeOffset now)
    {
        if (student.Role != UserRole.Student)
        {
            throw new ApiException(StatusCodes.Status403Forbidden, "Only students can submit answers.");
        }

        if (!student.ClassId.HasValue || student.ClassId.Value != assignment.ClassId)
        {
            throw new ApiException(StatusCodes.Status403Forbidden, "This assignment is not assigned to your class.");
        }

        if (assignment.Status != AssignmentStatus.Published)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "The assignment is not available.");
        }

        if (now > assignment.Deadline)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "The assignment deadline has passed.");
        }
    }
}
