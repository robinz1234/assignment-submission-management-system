# Database files

The primary database setup is handled by the Entity Framework Core migration in `backend/AssignmentManagement.Api/Migrations`. The API applies pending migrations and inserts demo data when it starts.

## Automatic setup

Run the project with Docker Compose from the repository root:

```bash
docker compose up --build
```

PostgreSQL is created automatically, then the API applies the migration and executes `DbSeeder`.

## Manual SQL setup

The file `schema-and-seed.sql` creates the schema, indexes, relationships, demo accounts, sample assignments, and a sample submission.

```bash
psql -U postgres -d assignment_management -f database/schema-and-seed.sql
```

Do not apply both the SQL script and the EF Core migration to a new database. Choose one method.

## Demo credentials

| Role | Email | Password |
|---|---|---|
| Admin | `admin@school.test` | `Admin123!` |
| Teacher | `teacher@school.test` | `Teacher123!` |
| Student | `student@school.test` | `Student123!` |

The SQL file contains only demonstration credentials. Replace them in any deployed environment.
