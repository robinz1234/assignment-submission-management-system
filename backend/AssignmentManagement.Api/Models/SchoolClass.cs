namespace AssignmentManagement.Api.Models;

public class SchoolClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;

    public ICollection<AppUser> Students { get; set; } = [];
    public ICollection<TeachingAssignment> TeachingAssignments { get; set; } = [];
    public ICollection<Assignment> Assignments { get; set; } = [];
}
