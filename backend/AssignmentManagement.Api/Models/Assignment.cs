namespace AssignmentManagement.Api.Models;

public class Assignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TeacherId { get; set; }
    public AppUser Teacher { get; set; } = null!;
    public Guid ClassId { get; set; }
    public SchoolClass Class { get; set; } = null!;
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset Deadline { get; set; }
    public decimal MaxMarks { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;
    public bool AllowResubmission { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Submission> Submissions { get; set; } = [];
}
