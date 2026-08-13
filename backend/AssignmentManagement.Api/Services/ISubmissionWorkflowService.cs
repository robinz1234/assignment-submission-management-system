using AssignmentManagement.Api.Models;

namespace AssignmentManagement.Api.Services;

public interface ISubmissionWorkflowService
{
    void EnsureStudentCanCreate(Assignment assignment, AppUser student, Submission? existingSubmission, DateTimeOffset now);
    void EnsureStudentCanUpdate(Assignment assignment, AppUser student, Submission submission, DateTimeOffset now);
    void EnsureTeacherCanReview(Assignment assignment, Submission submission, Guid teacherId, decimal? marks);
}
