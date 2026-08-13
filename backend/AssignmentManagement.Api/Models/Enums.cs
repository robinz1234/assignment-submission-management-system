namespace AssignmentManagement.Api.Models;

public enum UserRole
{
    Admin,
    Teacher,
    Student
}

public enum AssignmentStatus
{
    Draft,
    Published
}

public enum SubmissionStatus
{
    Submitted,
    Reviewed,
    Returned
}
