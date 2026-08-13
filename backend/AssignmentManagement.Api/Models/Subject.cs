namespace AssignmentManagement.Api.Models;

public class Subject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public ICollection<TeachingAssignment> TeachingAssignments { get; set; } = [];
    public ICollection<Assignment> Assignments { get; set; } = [];
}
