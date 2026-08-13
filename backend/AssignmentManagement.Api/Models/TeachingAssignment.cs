namespace AssignmentManagement.Api.Models;

public class TeachingAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TeacherId { get; set; }
    public AppUser Teacher { get; set; } = null!;
    public Guid ClassId { get; set; }
    public SchoolClass Class { get; set; } = null!;
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
