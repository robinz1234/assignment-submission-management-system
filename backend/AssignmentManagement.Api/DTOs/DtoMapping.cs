using AssignmentManagement.Api.Models;

namespace AssignmentManagement.Api.DTOs;

public static class DtoMapping
{
    public static UserDto ToDto(this AppUser user) => new(
        user.Id,
        user.FullName,
        user.Email,
        user.Role,
        user.ClassId,
        user.Class is null ? null : $"{user.Class.Name} - {user.Class.Section}",
        user.IsActive,
        user.CreatedAt);

    public static ClassDto ToDto(this SchoolClass schoolClass) => new(
        schoolClass.Id,
        schoolClass.Name,
        schoolClass.Section,
        schoolClass.AcademicYear,
        schoolClass.Students.Count);

    public static SubjectDto ToDto(this Subject subject) => new(subject.Id, subject.Name, subject.Code);

    public static TeachingAssignmentDto ToDto(this TeachingAssignment assignment) => new(
        assignment.Id,
        assignment.TeacherId,
        assignment.Teacher.FullName,
        assignment.ClassId,
        $"{assignment.Class.Name} - {assignment.Class.Section}",
        assignment.SubjectId,
        assignment.Subject.Name,
        assignment.CreatedAt);

    public static AssignmentDto ToDto(this Assignment assignment, Guid? studentId = null)
    {
        var mySubmission = studentId.HasValue
            ? assignment.Submissions.FirstOrDefault(item => item.StudentId == studentId.Value)
            : null;

        return new AssignmentDto(
            assignment.Id,
            assignment.TeacherId,
            assignment.Teacher.FullName,
            assignment.ClassId,
            $"{assignment.Class.Name} - {assignment.Class.Section}",
            assignment.SubjectId,
            assignment.Subject.Name,
            assignment.Title,
            assignment.Description,
            assignment.Deadline,
            assignment.MaxMarks,
            assignment.Status,
            assignment.AllowResubmission,
            assignment.Submissions.Count,
            mySubmission?.Id,
            mySubmission?.Status,
            mySubmission?.Marks,
            mySubmission?.Feedback,
            assignment.CreatedAt,
            assignment.UpdatedAt);
    }

    public static SubmissionDto ToDto(this Submission submission) => new(
        submission.Id,
        submission.AssignmentId,
        submission.Assignment.Title,
        submission.Assignment.MaxMarks,
        submission.StudentId,
        submission.Student.FullName,
        submission.AnswerText,
        submission.Status,
        submission.Marks,
        submission.Feedback,
        submission.SubmittedAt,
        submission.UpdatedAt,
        submission.ReviewedAt);
}
