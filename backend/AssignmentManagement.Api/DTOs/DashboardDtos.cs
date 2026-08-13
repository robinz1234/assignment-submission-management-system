namespace AssignmentManagement.Api.DTOs;

public record DashboardMetricDto(string Label, int Value, string Hint);

public record DashboardDto(
    string Role,
    IReadOnlyList<DashboardMetricDto> Metrics,
    IReadOnlyList<AssignmentDto> RecentAssignments,
    IReadOnlyList<SubmissionDto> RecentSubmissions);

public record SettingDto(int Id, string Key, string Value, string? Description, DateTimeOffset UpdatedAt);

public class UpdateSettingRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(1000)]
    public string Value { get; set; } = string.Empty;
}
