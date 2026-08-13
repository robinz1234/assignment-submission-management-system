using AssignmentManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<SchoolClass> Classes => Set<SchoolClass>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<TeachingAssignment> TeachingAssignments => Set<TeachingAssignment>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<AppSetting> Settings => Set<AppSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.FullName).HasMaxLength(120).IsRequired();
            entity.Property(item => item.Email).HasMaxLength(180).IsRequired();
            entity.Property(item => item.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(item => item.Role).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(item => item.Email).IsUnique();
            entity.HasOne(item => item.Class)
                .WithMany(item => item.Students)
                .HasForeignKey(item => item.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SchoolClass>(entity =>
        {
            entity.ToTable("classes");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(100).IsRequired();
            entity.Property(item => item.Section).HasMaxLength(50).IsRequired();
            entity.Property(item => item.AcademicYear).HasMaxLength(20).IsRequired();
            entity.HasIndex(item => new { item.Name, item.Section, item.AcademicYear }).IsUnique();
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("subjects");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).HasMaxLength(120).IsRequired();
            entity.Property(item => item.Code).HasMaxLength(30).IsRequired();
            entity.HasIndex(item => item.Code).IsUnique();
        });

        modelBuilder.Entity<TeachingAssignment>(entity =>
        {
            entity.ToTable("teaching_assignments");
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => new { item.TeacherId, item.ClassId, item.SubjectId }).IsUnique();
            entity.HasOne(item => item.Teacher)
                .WithMany(item => item.TeachingAssignments)
                .HasForeignKey(item => item.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.Class)
                .WithMany(item => item.TeachingAssignments)
                .HasForeignKey(item => item.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.Subject)
                .WithMany(item => item.TeachingAssignments)
                .HasForeignKey(item => item.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("assignments");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Title).HasMaxLength(180).IsRequired();
            entity.Property(item => item.Description).HasMaxLength(5000).IsRequired();
            entity.Property(item => item.MaxMarks).HasPrecision(8, 2);
            entity.Property(item => item.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(item => new { item.ClassId, item.SubjectId, item.Status, item.Deadline });
            entity.HasOne(item => item.Teacher)
                .WithMany(item => item.AssignmentsCreated)
                .HasForeignKey(item => item.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.Class)
                .WithMany(item => item.Assignments)
                .HasForeignKey(item => item.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.Subject)
                .WithMany(item => item.Assignments)
                .HasForeignKey(item => item.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.ToTable("submissions");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.AnswerText).HasMaxLength(10000).IsRequired();
            entity.Property(item => item.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(item => item.Marks).HasPrecision(8, 2);
            entity.Property(item => item.Feedback).HasMaxLength(3000);
            entity.HasIndex(item => new { item.AssignmentId, item.StudentId }).IsUnique();
            entity.HasOne(item => item.Assignment)
                .WithMany(item => item.Submissions)
                .HasForeignKey(item => item.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(item => item.Student)
                .WithMany(item => item.Submissions)
                .HasForeignKey(item => item.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AppSetting>(entity =>
        {
            entity.ToTable("settings");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Key).HasMaxLength(100).IsRequired();
            entity.Property(item => item.Value).HasMaxLength(1000).IsRequired();
            entity.Property(item => item.Description).HasMaxLength(500);
            entity.HasIndex(item => item.Key).IsUnique();
        });
    }
}
