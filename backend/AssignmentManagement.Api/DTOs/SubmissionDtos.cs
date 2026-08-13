using System.ComponentModel.DataAnnotations;
using AssignmentManagement.Api.Models;

namespace AssignmentManagement.Api.DTOs;

public record SubmissionDto(
    Guid Id,
    Guid AssignmentId,
    string AssignmentTitle,
    decimal MaxMarks,
    Guid StudentId,
    string StudentName,
    string AnswerText,
    SubmissionStatus Status,
    decimal? Marks,
    string? Feedback,
    DateTimeOffset SubmittedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ReviewedAt);

public class SubmitAnswerRequest
{
    [Required, StringLength(10000, MinimumLength = 1)]
    public string AnswerText { get; set; } = string.Empty;
}

public class ReviewSubmissionRequest
{
    [Range(0, 999999)]
    public decimal? Marks { get; set; }

    [StringLength(3000)]
    public string? Feedback { get; set; }

    public SubmissionStatus Status { get; set; } = SubmissionStatus.Reviewed;
}
