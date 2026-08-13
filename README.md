# Assignment & Submission Management System

A full-stack, role-based Assignment and Submission Management System developed for the **Assistant Software Engineer Recruitment Project**.

The application is designed for a school or college environment and provides separate workflows for **Administrators, Teachers, and Students**. Administrators manage academic structures and users, teachers create and evaluate assignments, and students view, submit, and track their academic work.

The system implements authentication, backend role-based authorization, assignment and submission workflows, relational database design, API documentation, validation, error handling, logging, and automated unit tests.

---

## 1. Main Features

### Administrator

Administrators can:

* Create and manage Admin, Teacher, and Student accounts.
* Activate or deactivate users.
* Manage classes or courses.
* Manage academic subjects.
* Assign teachers to specific class and subject combinations.
* View all assignments.
* View all student submissions.
* Manage application-level settings.
* Search and filter system data where supported.

### Teacher

Teachers can:

* Create assignments.
* Edit assignments.
* Delete assignments where allowed.
* Assign work to a specific class and subject.
* Define assignment title and description.
* Set assignment deadlines.
* Define maximum marks.
* Save assignments as drafts.
* Publish assignments for students.
* Allow or restrict submission updates.
* View student submissions.
* Review submitted work.
* Provide marks.
* Provide written feedback.
* Change submission status.

Teachers are restricted to the class and subject combinations assigned to them by an Administrator.

### Student

Students can:

* View published assignments assigned to their class.
* View assignment descriptions and deadlines.
* Submit text-based answers.
* Update submissions before the deadline when updates are allowed.
* View submission status.
* View awarded marks.
* View teacher feedback.

Students cannot access unpublished assignments or assignments belonging to other classes.

---

## 2. Technology Stack

| Layer               | Technology                       |
| ------------------- | -------------------------------- |
| Frontend            | Next.js 15                       |
| UI Library          | React 19                         |
| Language            | TypeScript                       |
| Styling             | Tailwind CSS                     |
| Form Handling       | React Hook Form                  |
| Validation          | Zod                              |
| Backend             | ASP.NET Core 8 Web API           |
| Backend Language    | C#                               |
| ORM                 | Entity Framework Core            |
| Database            | PostgreSQL 16                    |
| Authentication      | JWT Bearer Authentication        |
| Authorization       | Role-Based Backend Authorization |
| API Documentation   | Swagger / OpenAPI                |
| Unit Testing        | xUnit                            |
| Containerization    | Docker                           |
| Local Orchestration | Docker Compose                   |

---

## 3. Application Workflow

The core application workflow is:

```text
Administrator
      |
      |-- Creates Classes
      |-- Creates Subjects
      |-- Creates Teachers
      |-- Creates Students
      |
      `-- Assigns Teacher to Class + Subject
                         |
                         v
                      Teacher
                         |
                         |-- Creates Assignment
                         |-- Draft or Published
                         |
                         v
                      Student
                         |
                         |-- Views Published Assignment
                         |-- Submits Answer
                         |-- Updates Before Deadline if Allowed
                         |
                         v
                      Teacher
                         |
                         |-- Reviews Submission
                         |-- Awards Marks
                         |-- Provides Feedback
                         |
                         v
                      Student
                         |
                         `-- Views Status, Marks and Feedback
```

---

## 4. Project Structure

```text
Assignment_Submission_Management_System/
|
|-- .github/
|   `-- workflows/
|       `-- ci.yml
|
|-- backend/
|   |
|   |-- AssignmentManagement.Api/
|   |   |-- Controllers/
|   |   |-- Data/
|   |   |-- DTOs/
|   |   |-- Models/
|   |   |-- Services/
|   |   |-- Migrations/
|   |   |-- Program.cs
|   |   |-- appsettings.json
|   |   |-- AssignmentManagement.Api.csproj
|   |   `-- Dockerfile
|   |
|   `-- AssignmentManagement.Tests/
|       |-- AuthorizationAttributeTests.cs
|       |-- PasswordHasherTests.cs
|       |-- SubmissionWorkflowServiceTests.cs
|       `-- AssignmentManagement.Tests.csproj
|
|-- frontend/
|   |-- src/
|   |-- public/
|   |-- package.json
|   |-- tailwind.config.ts
|   |-- tsconfig.json
|   |-- .env.local.example
|   `-- Dockerfile
|
|-- database/
|   |-- schema-and-seed.sql
|   `-- README.md
|
|-- docs/
|   |-- API_REFERENCE.md
|   |-- OPERATING_GUIDE.md
|   |-- SUBMISSION_CHECKLIST.md
|   |-- ASSESSMENT_REQUIREMENTS_MATRIX.md
|   |-- GITHUB_AND_SUBMISSION_GUIDE.md
|   |
|   `-- diagrams/
|       |-- ERD.drawio
|       |-- ERD.svg
|       |-- ERD.mmd
|       |-- ARCHITECTURE.drawio
|       |-- ARCHITECTURE.svg
|       `-- ARCHITECTURE.mmd
|
|-- .env.example
|-- .gitignore
|-- docker-compose.yml
|-- AssignmentSubmission.sln
|-- START_HERE.md
|-- start-docker.ps1
|-- reset-docker.ps1
|-- verify-project.ps1
`-- README.md
```

---

## 5. Demo Credentials

The application automatically creates demo accounts during database initialization.

### Administrator

```text
Email: admin@school.test
Password: Admin123!
```

### Teacher

```text
Email: teacher@school.test
Password: Teacher123!
```

### Student

```text
Email: student@school.test
Password: Student123!
```

### Additional Student

```text
Email: student2@school.test
Password: Student123!
```

These accounts are intended only for demonstration and assessment purposes.

No real account credentials or production secrets are included in this repository.

---

## 6. Recommended Setup Using Docker

Docker Compose is the recommended method because it starts the PostgreSQL database, ASP.NET Core API, and Next.js frontend together.

### Prerequisites

Install:

* Git
* Docker Desktop
* WSL 2 when using the Docker Desktop WSL backend on Windows

Make sure Docker Desktop is running before starting the project.

---

## 7. Clone and Start the Project

Clone the repository:

```bash
git clone YOUR_REPOSITORY_URL
cd assignment-submission-management-system
```

### Windows Command Prompt

Create the local environment file:

```cmd
copy .env.example .env
```

Start the application:

```cmd
docker compose up --build
```

### Windows PowerShell

```powershell
Copy-Item .env.example .env
docker compose up --build
```

Alternatively:

```powershell
.\start-docker.ps1
```

### macOS / Linux

```bash
cp .env.example .env
chmod +x start-docker.sh reset-docker.sh
./start-docker.sh
```

The first Docker build may take several minutes while required images and dependencies are downloaded.

---

## 8. Application URLs

After the Docker services start successfully:

### Frontend

```text
http://localhost:3000
```

### Swagger / OpenAPI

```text
http://localhost:5050/swagger
```

### API Health Check

```text
http://localhost:5050/health
```

### PostgreSQL

```text
Host: localhost
Port: 5432
Database: assignment_management
```

The ASP.NET Core API listens internally on port `8080` inside its Docker container. Docker Compose maps that internal port to port `5050` on the host computer.

---

## 9. Docker Services

The Docker Compose configuration starts three main services:

```text
assignment-database
assignment-api
assignment-web
```

Their responsibilities are:

| Service               | Purpose               |
| --------------------- | --------------------- |
| `assignment-database` | PostgreSQL database   |
| `assignment-api`      | ASP.NET Core REST API |
| `assignment-web`      | Next.js frontend      |

Check the running services with:

```bash
docker compose ps
```

Stop the application with:

```bash
docker compose down
```

---

## 10. Database Setup

PostgreSQL is used because the application contains strongly related academic data such as users, classes, subjects, teacher assignments, assignments, and student submissions.

When the API starts, Entity Framework Core automatically applies the required database migration.

The application also inserts sample data so that an evaluator can immediately test the system without manually creating database tables or demo accounts.

The repository also includes:

```text
database/schema-and-seed.sql
```

This provides an alternative database setup reference.

### Important

For a new database, use either:

1. Entity Framework migrations and automatic C# seed data, which is the recommended method.

or

2. `database/schema-and-seed.sql`.

Do not initialize the same empty database using both approaches.

---

## 11. Resetting the Database

To remove the Docker database volume and recreate the original seeded data:

### Windows PowerShell

```powershell
.\reset-docker.ps1
```

### macOS / Linux

```bash
./reset-docker.sh
```

This should only be used when a complete local database reset is required.

---

## 12. Manual Setup Without Docker

Docker is recommended, but the frontend and backend can also be run manually.

### Requirements

Install:

* .NET SDK 8
* Node.js 20 or later
* PostgreSQL 16 or a compatible PostgreSQL version

### Database

Create a PostgreSQL database:

```bash
createdb -U postgres assignment_management
```

### Backend

Navigate to:

```bash
cd backend/AssignmentManagement.Api
```

Restore dependencies:

```bash
dotnet restore
```

Run the API:

```bash
dotnet run
```

The API applies the database migration and seed data during startup.

### Frontend

Open another terminal:

```bash
cd frontend
```

Create the frontend environment file.

Windows PowerShell:

```powershell
Copy-Item .env.local.example .env.local
```

macOS / Linux:

```bash
cp .env.local.example .env.local
```

Install dependencies:

```bash
npm install
```

Run the development server:

```bash
npm run dev
```

Open:

```text
http://localhost:3000
```

---

## 13. Environment Configuration

The repository contains:

```text
.env.example
```

Copy this file to:

```text
.env
```

for local Docker usage.

The main variables are:

| Variable                   | Purpose                          |
| -------------------------- | -------------------------------- |
| `POSTGRES_DB`              | PostgreSQL database name         |
| `POSTGRES_USER`            | PostgreSQL username              |
| `POSTGRES_PASSWORD`        | Local PostgreSQL password        |
| `JWT_KEY`                  | JWT signing key                  |
| `NEXT_PUBLIC_API_BASE_URL` | API address used by the frontend |

The `.env` file is excluded by `.gitignore` and should not be committed.

The `.env.example` file is intentionally committed so evaluators can see which variables are required.

---

## 14. Authentication and Authorization

The application uses JWT-based authentication.

After successful login, the backend creates a signed token containing the authenticated user's identity and role.

The API enforces authorization for:

* Admin
* Teacher
* Student

Role restrictions are implemented in the backend and are not dependent only on hiding frontend navigation items.

Additional business-level checks verify teacher ownership, teaching assignments, student class membership, assignment publication status, submission deadlines, and review permissions.

---

## 15. Password Security

Passwords are not stored as plain text.

The backend hashes passwords using:

```text
PBKDF2
SHA-256
Random Salt
100,000 Iterations
```

Only password hashes and required verification information are stored in the database.

---

## 16. Important Business Rules

The application implements the following business rules:

1. Draft assignments are not visible to students.

2. Teachers can create assignments only for class and subject combinations assigned to them by an Administrator.

3. Students can view only published assignments assigned to their class.

4. Students cannot submit assignments after the deadline.

5. A student can have only one current submission for an assignment.

6. A submission can be updated only before the deadline and only when submission updates are enabled.

7. Reviewed submissions cannot be modified by the Student.

8. Teachers can review only submissions belonging to their assignments.

9. Awarded marks cannot exceed the assignment's maximum marks.

10. Marks cannot be negative.

11. Important academic records are preserved when users are deactivated.

12. An assignment with existing submissions cannot have its class or subject changed in ways that would invalidate the submission records.

---

## 17. Unit Testing

The backend contains an xUnit test project:

```text
backend/AssignmentManagement.Tests
```

The tests cover important application behavior including:

* Submission deadline validation.
* Student class access restrictions.
* Resubmission and update rules.
* Maximum marks validation.
* Teacher review workflow.
* Role-based authorization attributes.
* Password hashing behavior.

### Run Tests with .NET SDK

From the repository root:

```bash
dotnet test AssignmentSubmission.sln
```

### Run Tests Through Docker

This is useful when the .NET SDK is not installed directly on the host computer.

Windows Command Prompt:

```cmd
docker run --rm -v "%cd%:/src" -w /src mcr.microsoft.com/dotnet/sdk:8.0 dotnet test AssignmentSubmission.sln
```

A successful run should complete with:

```text
Failed: 0
```

---

## 18. Frontend Build

To verify the frontend production build manually:

```bash
cd frontend
npm install
npm run build
```

The frontend is also built automatically when running:

```bash
docker compose up --build
```

---

## 19. API Documentation

Interactive Swagger documentation is available while the API is running:

```text
http://localhost:5050/swagger
```

A written endpoint reference is also available at:

```text
docs/API_REFERENCE.md
```

Swagger can be used to inspect the available REST endpoints, request models, response models, and authentication requirements.

---

## 20. Error Handling and Logging

The ASP.NET Core backend includes centralized API error handling and application logging.

Validation errors and business-rule violations return appropriate API responses rather than exposing internal application details to the frontend.

ASP.NET Core logging is used for application lifecycle information, database operations, and relevant API events.

---

## 21. Form Validation

Frontend forms use structured validation before API requests are submitted.

The application uses:

```text
React Hook Form
+
Zod
```

The backend also independently validates important rules so that API security and business logic do not depend on frontend validation alone.

---

## 22. Data Model and Relationships

The system uses a relational model.

Important relationships include:

```text
Class
  |
  `-- Students

Teacher
  |
  `-- Teaching Assignments
          |
          |-- Class
          `-- Subject

Teacher
  |
  `-- Assignments
          |
          |-- Class
          |-- Subject
          `-- Submissions
                    |
                    `-- Student
```

`TeachingAssignment` represents the relationship between a Teacher, Class, and Subject.

`Submission` contains a unique relationship between an Assignment and Student so that one Student cannot create multiple independent submissions for the same Assignment.

---

## 23. ERD and Architecture Diagrams

Editable diagrams are included in:

```text
docs/diagrams/
```

Available files include:

```text
ERD.drawio
ERD.svg
ERD.mmd

ARCHITECTURE.drawio
ARCHITECTURE.svg
ARCHITECTURE.mmd
```

The `.drawio` files can be edited using diagrams.net.

The SVG files provide quick previews, while the Mermaid files provide source-based diagram representations.

---

## 24. Assumptions

The following assumptions were made where the assessment did not explicitly define behavior:

* Each Student belongs to one class or course at a time.
* A Teacher may teach multiple class and subject combinations.
* Assignment submissions are text-based.
* File uploads are outside the required project scope.
* A Student has one active submission per Assignment.
* Updating an existing submission is treated as resubmission.
* A reviewed submission becomes locked from Student editing.
* Administrators can view all assignments and submissions but do not grade Student work.
* Dates are stored consistently and displayed according to the client environment.
* User removal is implemented through deactivation where academic history should be preserved.

---

## 25. Known Limitations

The current assessment implementation does not include:

* Password reset by email.
* Email verification.
* File attachment submissions.
* Real-time notifications.
* Refresh-token rotation.
* Production email services.
* Production cloud deployment.
* Advanced learning-management features outside the assessment scope.

The application currently stores the authentication token in browser storage for the local assessment environment. A production system should use a hardened authentication strategy, HTTPS, secure secret management, and additional security controls.

---

## 26. Design Decisions

### PostgreSQL

PostgreSQL was selected because the system contains clearly defined relational entities and constraints.

### Teaching Assignment Entity

A dedicated `TeachingAssignment` model is used to define which Teacher is permitted to work with each Class and Subject combination.

### Submission Constraint

A unique Assignment and Student relationship prevents duplicate submission records while still allowing an existing submission to be updated when permitted.

### Backend Business Logic

Important academic rules are validated by the API instead of relying only on the frontend.

### DTO Usage

API DTOs prevent sensitive or unnecessary internal model information from being directly exposed.

### Docker Compose

Docker Compose provides a consistent evaluator setup for the frontend, backend, and PostgreSQL database with minimal manual configuration.

---

## 27. Security Notes

* JWT-based authentication is implemented.
* Role-based authorization is enforced by backend API endpoints.
* Passwords are hashed before database storage.
* Real production credentials are not included.
* `.env` is excluded from version control.
* `.env.example` documents required configuration.
* Student access is restricted according to class membership.
* Teacher actions are restricted according to assigned teaching scope.
* Server-side validation protects important business rules.

---

## 28. Assessment Requirement Coverage

| Assessment Requirement   | Implementation |
| ------------------------ | -------------- |
| Next.js                  | Implemented    |
| React                    | Implemented    |
| TypeScript               | Implemented    |
| Responsive UI            | Implemented    |
| Form Validation          | Implemented    |
| API Integration          | Implemented    |
| ASP.NET Core Web API     | Implemented    |
| C#                       | Implemented    |
| RESTful API              | Implemented    |
| Backend Validation       | Implemented    |
| Error Handling           | Implemented    |
| Logging                  | Implemented    |
| Swagger / OpenAPI        | Implemented    |
| PostgreSQL               | Implemented    |
| JWT Authentication       | Implemented    |
| Role-Based Authorization | Implemented    |
| Database Relationships   | Implemented    |
| Migration Files          | Included       |
| Seed / Sample Data       | Included       |
| Unit Tests               | Included       |
| Docker Configuration     | Included       |
| Environment Example      | Included       |
| Editable ERD             | Included       |
| Setup Documentation      | Included       |
| Demo Credentials         | Included       |

---

## 29. Quick Evaluation Guide

For a quick demonstration:

### Administrator

Login with:

```text
admin@school.test
Admin123!
```

Verify:

* User management.
* Classes and subjects.
* Teacher assignments.
* All assignments.
* All submissions.
* Application settings.

### Teacher

Login with:

```text
teacher@school.test
Teacher123!
```

Verify:

* Assignment creation.
* Draft and Published status.
* Assignment editing.
* Student submission review.
* Marks and feedback.

### Student

Login with:

```text
student@school.test
Student123!
```

Verify:

* Published assignments.
* Assignment details.
* Submission workflow.
* Submission update rules.
* Marks and teacher feedback.

---

## 30. Additional Documentation

Additional project documentation is available under:

```text
docs/
```

Important files include:

```text
API_REFERENCE.md
OPERATING_GUIDE.md
SUBMISSION_CHECKLIST.md
ASSESSMENT_REQUIREMENTS_MATRIX.md
GITHUB_AND_SUBMISSION_GUIDE.md
```

---

## 31. Final Submission Checklist

Before submitting the repository:

* Confirm the repository is accessible to the evaluator.
* Confirm both frontend and backend source code are present.
* Confirm `.env` is not committed.
* Confirm `.env.example` is committed.
* Confirm database migrations and seed data are present.
* Confirm Admin, Teacher, and Student demo accounts work.
* Confirm Docker setup works.
* Confirm Swagger opens correctly.
* Confirm unit tests complete successfully.
* Confirm role-based access is enforced.
* Confirm the README renders correctly on GitHub.

---

