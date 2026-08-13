using AssignmentManagement.Api.Models;
using AssignmentManagement.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Api.Data;

public class DbSeeder(AppDbContext dbContext, IPasswordHasher passwordHasher)
{
    public async Task SeedAsync()
    {
        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        var classTen = new SchoolClass
        {
            Name = "Class 10",
            Section = "A",
            AcademicYear = "2026"
        };

        var mathematics = new Subject { Name = "Mathematics", Code = "MATH-10" };
        var physics = new Subject { Name = "Physics", Code = "PHY-10" };
        var english = new Subject { Name = "English", Code = "ENG-10" };

        var admin = new AppUser
        {
            FullName = "System Administrator",
            Email = "admin@school.test",
            PasswordHash = passwordHasher.Hash("Admin123!"),
            Role = UserRole.Admin
        };

        var teacher = new AppUser
        {
            FullName = "Nadia Rahman",
            Email = "teacher@school.test",
            PasswordHash = passwordHasher.Hash("Teacher123!"),
            Role = UserRole.Teacher
        };

        var student = new AppUser
        {
            FullName = "Ayan Ahmed",
            Email = "student@school.test",
            PasswordHash = passwordHasher.Hash("Student123!"),
            Role = UserRole.Student,
            Class = classTen
        };

        var secondStudent = new AppUser
        {
            FullName = "Maliha Islam",
            Email = "student2@school.test",
            PasswordHash = passwordHasher.Hash("Student123!"),
            Role = UserRole.Student,
            Class = classTen
        };

        dbContext.AddRange(classTen, mathematics, physics, english, admin, teacher, student, secondStudent);

        var teachingAssignment = new TeachingAssignment
        {
            Teacher = teacher,
            Class = classTen,
            Subject = mathematics
        };
        dbContext.TeachingAssignments.Add(teachingAssignment);

        var publishedAssignment = new Assignment
        {
            Teacher = teacher,
            Class = classTen,
            Subject = mathematics,
            Title = "Quadratic Equations Practice",
            Description = "Solve the five quadratic equation problems and explain the method used for each answer.",
            Deadline = DateTimeOffset.UtcNow.AddDays(7),
            MaxMarks = 20,
            Status = AssignmentStatus.Published,
            AllowResubmission = true
        };

        var draftAssignment = new Assignment
        {
            Teacher = teacher,
            Class = classTen,
            Subject = mathematics,
            Title = "Geometry Revision",
            Description = "A draft assignment prepared for the next lesson.",
            Deadline = DateTimeOffset.UtcNow.AddDays(14),
            MaxMarks = 15,
            Status = AssignmentStatus.Draft,
            AllowResubmission = true
        };

        dbContext.Assignments.AddRange(publishedAssignment, draftAssignment);
        dbContext.Submissions.Add(new Submission
        {
            Assignment = publishedAssignment,
            Student = secondStudent,
            AnswerText = "I solved the equations by factorization and checked the roots by substitution.",
            Status = SubmissionStatus.Submitted
        });

        dbContext.Settings.AddRange(
            new AppSetting
            {
                Key = "SchoolName",
                Value = "Demo School and College",
                Description = "Name shown in the application header."
            },
            new AppSetting
            {
                Key = "DefaultAllowResubmission",
                Value = "true",
                Description = "Default resubmission preference for new assignments."
            });

        await dbContext.SaveChangesAsync();
    }
}
