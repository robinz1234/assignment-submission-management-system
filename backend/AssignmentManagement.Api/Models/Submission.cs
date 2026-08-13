namespace AssignmentManagement.Api.Models;

public class Submission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;
    public Guid StudentId { get; set; }
    public AppUser Student { get; set; } = null!;
    public string AnswerText { get; set; } = string.Empty;
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;
    public decimal? Marks { get; set; }
    public string? Feedback { get; set; }
    public DateTimeOffset SubmittedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ReviewedAt { get; set; }
}
