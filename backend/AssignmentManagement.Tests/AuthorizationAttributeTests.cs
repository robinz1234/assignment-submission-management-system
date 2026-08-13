using System.Reflection;
using AssignmentManagement.Api.Controllers;
using AssignmentManagement.Api.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;

namespace AssignmentManagement.Tests;

public class AuthorizationAttributeTests
{
    [Fact]
    public void AdminControllersRequireAdminRole()
    {
        AssertRole(typeof(UsersController), "Admin");
        AssertRole(typeof(ClassesController), "Admin");
        AssertRole(typeof(SubjectsController), "Admin");
        AssertRole(typeof(TeachingAssignmentsController), "Admin");
        AssertRole(typeof(SettingsController), "Admin");
    }

    [Fact]
    public void AssignmentCreateRequiresTeacherRole()
    {
        var method = typeof(AssignmentsController).GetMethod(nameof(AssignmentsController.Create));
        var attribute = method?.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal("Teacher", attribute!.Roles);
    }

    [Fact]
    public void SubmissionListRequiresTeacherOrAdminRole()
    {
        var method = typeof(SubmissionsController).GetMethod(nameof(SubmissionsController.GetAll));
        var attribute = method?.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal("Teacher,Admin", attribute!.Roles);
    }

    [Fact]
    public void SubmissionReviewRequiresTeacherRole()
    {
        var method = typeof(SubmissionsController).GetMethod(nameof(SubmissionsController.Review));
        var attribute = method?.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal("Teacher", attribute!.Roles);
    }

    private static void AssertRole(Type controllerType, string role)
    {
        var attribute = controllerType.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal(role, attribute!.Roles);
    }
}
