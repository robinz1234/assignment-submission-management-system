using System.ComponentModel.DataAnnotations;
using AssignmentManagement.Api.Models;

namespace AssignmentManagement.Api.DTOs;

public record AssignmentDto(
    Guid Id,
    Guid TeacherId,
    string TeacherName,
    Guid ClassId,
    string ClassName,
    Guid SubjectId,
    string SubjectName,
    string Title,
    string Description,
    DateTimeOffset Deadline,
    decimal MaxMarks,
    AssignmentStatus Status,
    bool AllowResubmission,
    int SubmissionCount,
    Guid? MySubmissionId,
    SubmissionStatus? MySubmissionStatus,
    decimal? MyMarks,
    string? MyFeedback,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public class CreateAssignmentRequest
{
    [Required]
    public Guid ClassId { get; set; }

    [Required]
    public Guid SubjectId { get; set; }

    [Required, StringLength(180, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(5000, MinimumLength = 3)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset Deadline { get; set; }

    [Range(0.01, 999999)]
    public decimal MaxMarks { get; set; }

    public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;
    public bool AllowResubmission { get; set; } = true;
}

public class UpdateAssignmentRequest : CreateAssignmentRequest
{
}
