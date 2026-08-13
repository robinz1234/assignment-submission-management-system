using System.ComponentModel.DataAnnotations;

namespace AssignmentManagement.Api.DTOs;

public record ClassDto(Guid Id, string Name, string Section, string AcademicYear, int StudentCount);

public class UpsertClassRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50, MinimumLength = 1)]
    public string Section { get; set; } = string.Empty;

    [Required, StringLength(20, MinimumLength = 4)]
    public string AcademicYear { get; set; } = string.Empty;
}

public record SubjectDto(Guid Id, string Name, string Code);

public class UpsertSubjectRequest
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(30, MinimumLength = 2)]
    public string Code { get; set; } = string.Empty;
}

public record TeachingAssignmentDto(
    Guid Id,
    Guid TeacherId,
    string TeacherName,
    Guid ClassId,
    string ClassName,
    Guid SubjectId,
    string SubjectName,
    DateTimeOffset CreatedAt);

public class CreateTeachingAssignmentRequest
{
    [Required]
    public Guid TeacherId { get; set; }

    [Required]
    public Guid ClassId { get; set; }

    [Required]
    public Guid SubjectId { get; set; }
}
