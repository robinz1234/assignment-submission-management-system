namespace AssignmentManagement.Api.Models;

public class AppUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Guid? ClassId { get; set; }
    public SchoolClass? Class { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<TeachingAssignment> TeachingAssignments { get; set; } = [];
    public ICollection<Assignment> AssignmentsCreated { get; set; } = [];
    public ICollection<Submission> Submissions { get; set; } = [];
}
