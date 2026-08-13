using AssignmentManagement.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AssignmentManagement.Api.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("202608050001_InitialCreate")]
public class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "classes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Section = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                AcademicYear = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_classes", item => item.Id));

        migrationBuilder.CreateTable(
            name: "settings",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_settings", item => item.Id));

        migrationBuilder.CreateTable(
            name: "subjects",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_subjects", item => item.Id));

        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                FullName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Email = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                ClassId = table.Column<Guid>(type: "uuid", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_users", item => item.Id);
                table.ForeignKey(
                    name: "FK_users_classes_ClassId",
                    column: item => item.ClassId,
                    principalTable: "classes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "assignments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                Deadline = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                MaxMarks = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                AllowResubmission = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_assignments", item => item.Id);
                table.ForeignKey("FK_assignments_classes_ClassId", item => item.ClassId, "classes", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_assignments_subjects_SubjectId", item => item.SubjectId, "subjects", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_assignments_users_TeacherId", item => item.TeacherId, "users", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "teaching_assignments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_teaching_assignments", item => item.Id);
                table.ForeignKey("FK_teaching_assignments_classes_ClassId", item => item.ClassId, "classes", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_teaching_assignments_subjects_SubjectId", item => item.SubjectId, "subjects", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_teaching_assignments_users_TeacherId", item => item.TeacherId, "users", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "submissions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                AnswerText = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Marks = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                Feedback = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                SubmittedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ReviewedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_submissions", item => item.Id);
                table.ForeignKey("FK_submissions_assignments_AssignmentId", item => item.AssignmentId, "assignments", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_submissions_users_StudentId", item => item.StudentId, "users", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex("IX_assignments_ClassId_SubjectId_Status_Deadline", "assignments", new[] { "ClassId", "SubjectId", "Status", "Deadline" });
        migrationBuilder.CreateIndex("IX_assignments_SubjectId", "assignments", "SubjectId");
        migrationBuilder.CreateIndex("IX_assignments_TeacherId", "assignments", "TeacherId");
        migrationBuilder.CreateIndex("IX_classes_Name_Section_AcademicYear", "classes", new[] { "Name", "Section", "AcademicYear" }, unique: true);
        migrationBuilder.CreateIndex("IX_settings_Key", "settings", "Key", unique: true);
        migrationBuilder.CreateIndex("IX_subjects_Code", "subjects", "Code", unique: true);
        migrationBuilder.CreateIndex("IX_submissions_AssignmentId_StudentId", "submissions", new[] { "AssignmentId", "StudentId" }, unique: true);
        migrationBuilder.CreateIndex("IX_submissions_StudentId", "submissions", "StudentId");
        migrationBuilder.CreateIndex("IX_teaching_assignments_ClassId", "teaching_assignments", "ClassId");
        migrationBuilder.CreateIndex("IX_teaching_assignments_SubjectId", "teaching_assignments", "SubjectId");
        migrationBuilder.CreateIndex("IX_teaching_assignments_TeacherId_ClassId_SubjectId", "teaching_assignments", new[] { "TeacherId", "ClassId", "SubjectId" }, unique: true);
        migrationBuilder.CreateIndex("IX_users_ClassId", "users", "ClassId");
        migrationBuilder.CreateIndex("IX_users_Email", "users", "Email", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("settings");
        migrationBuilder.DropTable("submissions");
        migrationBuilder.DropTable("teaching_assignments");
        migrationBuilder.DropTable("assignments");
        migrationBuilder.DropTable("subjects");
        migrationBuilder.DropTable("users");
        migrationBuilder.DropTable("classes");
    }
}
