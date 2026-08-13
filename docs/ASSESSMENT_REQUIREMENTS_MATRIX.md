# Assessment requirements matrix

| Assessment requirement | Implementation |
|---|---|
| Role-based school or college system | Admin, Teacher, and Student roles across API and UI |
| Admin manages users | `/admin/users` API and Admin Users page |
| Admin manages classes and subjects | Admin Academic Setup page and APIs |
| Admin assigns teachers | `teaching_assignments` relationship, API, and UI |
| Admin views all assignments and submissions | Role-scoped list endpoints and navigation |
| Admin manages settings | Settings table, API, and Admin Settings page |
| Teacher assignment CRUD | Assignment API and teacher pages |
| Class and subject assignment | Validated teaching scope selection |
| Title, description, deadline, maximum marks | Validated request DTO and form |
| Draft and publish | Assignment status and publish endpoint |
| Teacher reviews submissions | Submission list and review form |
| Marks, feedback, and status | Review request and workflow service |
| Student views class assignments | API filters by current student's class and Published status |
| Student submits and updates | Create and update submission endpoints and UI |
| Deadline enforcement | Submission workflow service and tests |
| Student sees status, marks, feedback | Assignment detail and submissions pages |
| Next.js, React, TypeScript | `frontend` application |
| Responsive UI | Tailwind responsive layout and mobile navigation |
| Form validation | Data annotations, React Hook Form, and Zod |
| ASP.NET Core Web API and C# | `backend/AssignmentManagement.Api` |
| REST, validation, error handling, logging | Controllers, DTO attributes, middleware, built-in logging |
| Swagger and OpenAPI | Swagger configuration in `Program.cs` |
| PostgreSQL relationships | EF Core model, migration, and SQL script |
| Login and JWT | Auth controller and JWT service |
| Role authorization | Controller attributes plus resource ownership checks |
| Unit tests | xUnit tests for workflows, hashing, and authorization |
| Migration files | `backend/AssignmentManagement.Api/Migrations` |
| Seed or sample data | `DbSeeder.cs` and `database/schema-and-seed.sql` |
| README and setup instructions | Root `README.md` and operation guide |
| Demo credentials | README and login page |
| Environment example | Root `.env.example` and frontend `.env.local.example` |
| Optional Docker | Dockerfiles, Compose, and helper scripts |
| Optional pagination and filtering | Assignment, submission, and user list endpoints |
