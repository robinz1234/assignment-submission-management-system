# Assignment and Submission Management System

A complete role-based school or college application built for the Assistant Software Engineer recruitment project. Admins manage users and academic setup, teachers create and review assignments, and students submit answers and view marks and feedback.

## Main features

### Admin

- Create, update, deactivate, search, and filter users.
- Manage classes or courses and subjects.
- assign teachers to a class and subject combination.
- View all assignments and all submissions.
- Manage application settings.

### Teacher

- Create, edit, publish, and delete assignments.
- Work only within class and subject scopes assigned by an admin.
- Set title, description, deadline, maximum marks, status, and resubmission policy.
- View student submissions.
- Set marks, feedback, and submission status.

### Student

- View published assignments for the student's class.
- View assignment details and deadlines.
- Submit an answer.
- Update an answer before the deadline when resubmission is allowed.
- View submission status, marks, and feedback.

## Technology stack

| Layer | Technology |
|---|---|
| Frontend | Next.js 15, React 19, TypeScript, Tailwind CSS, React Hook Form, Zod |
| Backend | ASP.NET Core 8 Web API, C#, Entity Framework Core |
| Database | PostgreSQL 16 |
| Authentication | JWT bearer authentication and role-based authorization |
| API documentation | Swagger and OpenAPI |
| Testing | xUnit |
| Local orchestration | Docker Compose |

## Project structure

```text
.
|-- backend/
|   |-- AssignmentManagement.Api/       ASP.NET Core API
|   `-- AssignmentManagement.Tests/     xUnit tests
|-- frontend/                            Next.js application
|-- database/                            SQL schema and sample data
|-- docs/                                Design, API, diagrams, and guides
|-- docker-compose.yml                   One-command local setup
|-- AssignmentSubmission.sln             .NET solution
|-- .env.example                         Safe environment template
`-- README.md
```

## Demo credentials

| Role | Email | Password |
|---|---|---|
| Admin | `admin@school.test` | `Admin123!` |
| Teacher | `teacher@school.test` | `Teacher123!` |
| Student | `student@school.test` | `Student123!` |
| Additional student | `student2@school.test` | `Student123!` |

## Recommended setup, Docker Desktop

This is the easiest method because it starts PostgreSQL, the API, and the frontend together.

### Prerequisites

1. Install Docker Desktop.
2. Start Docker Desktop and wait until the Docker engine is running.
3. Extract this project ZIP to a simple path, for example `C:\Projects\Assignment_Submission_Management_System`.

### Start the project on Windows

Open PowerShell inside the extracted folder and run:

```powershell
Copy-Item .env.example .env
docker compose up --build
```

You may also run:

```powershell
.\start-docker.ps1
```

### Start the project on macOS or Linux

```bash
cp .env.example .env
chmod +x start-docker.sh reset-docker.sh
./start-docker.sh
```

### Open the application

- Frontend: `http://localhost:3000`
- Swagger API: `http://localhost:5050/swagger`
- API health endpoint: `http://localhost:5050/health`
- PostgreSQL: `localhost:5432`

### Stop the project

Press `Ctrl + C`, then run:

```bash
docker compose down
```

### Reset the database

This deletes the Docker database volume and recreates all sample data.

```powershell
.\reset-docker.ps1
```

or:

```bash
./reset-docker.sh
```

## Manual setup without Docker

### Prerequisites

- .NET SDK 8
- Node.js 20 or newer
- PostgreSQL 16 or a compatible PostgreSQL version

### 1. Create the database

Using PostgreSQL command line:

```bash
createdb -U postgres assignment_management
```

The default local database password used by the sample configuration is `postgres`. Change the connection string when your PostgreSQL credentials differ.

### 2. Start the backend

Open a terminal in the repository root:

```bash
cd backend/AssignmentManagement.Api
dotnet restore
dotnet run
```

The API applies the migration and creates demo data automatically. Swagger is available at `http://localhost:5050/swagger` when the launch profile is used.

For a custom database connection, set the environment variable before running:

PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=assignment_management;Username=postgres;Password=YOUR_PASSWORD"
$env:Jwt__Key="YOUR_LOCAL_DEVELOPMENT_KEY_WITH_AT_LEAST_32_CHARACTERS"
dotnet run
```

Bash:

```bash
export ConnectionStrings__DefaultConnection='Host=localhost;Port=5432;Database=assignment_management;Username=postgres;Password=YOUR_PASSWORD'
export Jwt__Key='YOUR_LOCAL_DEVELOPMENT_KEY_WITH_AT_LEAST_32_CHARACTERS'
dotnet run
```

### 3. Start the frontend

Open a second terminal:

```bash
cd frontend
cp .env.local.example .env.local
npm install
npm run dev
```

On Windows PowerShell, use:

```powershell
cd frontend
Copy-Item .env.local.example .env.local
npm install
npm run dev
```

Open `http://localhost:3000`.

## Run tests

From the repository root:

```bash
dotnet test AssignmentSubmission.sln
```

The test project covers key submission deadlines, class access, resubmission rules, maximum marks, password hashing, and role authorization attributes.

To build the frontend:

```bash
cd frontend
npm install
npm run build
```

To run all verification steps on Windows:

```powershell
.\verify-project.ps1
```

On macOS or Linux:

```bash
./verify-project.sh
```

## Database setup options

Use one of these methods for a new database:

1. Recommended: start the API and let EF Core apply the migration and C# seed data.
2. Alternative: run `database/schema-and-seed.sql` manually.

Do not use both methods on the same empty database.

## Authentication and authorization design

- Login returns a signed JWT containing user ID, email, name, and role claims.
- Controllers enforce `Admin`, `Teacher`, or `Student` roles on the backend.
- Teacher ownership and teaching scope are checked in business logic, not only in the UI.
- Student class membership, assignment publication, deadline, and resubmission rules are enforced by the API.
- Passwords are stored with PBKDF2 SHA-256, a random salt, and 100,000 iterations.

## Important business rules

- Draft assignments are visible only to the creating teacher and admins.
- A teacher can create an assignment only for a class and subject assigned by an admin.
- A student sees only published assignments for the student's class.
- A student cannot submit after the deadline.
- A student has at most one submission per assignment.
- An existing submission can be updated only before the deadline, when resubmission is enabled, and before it is reviewed.
- A teacher can review only submissions for the teacher's assignments.
- Marks must be between zero and the assignment's maximum marks.
- An assignment with submissions cannot have its class or subject changed.

## API documentation

Run the API and open Swagger at `http://localhost:5050/swagger`. A written endpoint summary is available in `docs/API_REFERENCE.md`.

## Editable diagrams

- `docs/diagrams/ERD.drawio`
- `docs/diagrams/ARCHITECTURE.drawio`
- `docs/diagrams/ERD.mmd`
- `docs/diagrams/ARCHITECTURE.mmd`

Open `.drawio` files in diagrams.net. The SVG files in the same folder are quick previews.

## Environment configuration

Copy `.env.example` to `.env` for Docker Compose. No real credentials or production secrets are included.

| Variable | Purpose |
|---|---|
| `POSTGRES_DB` | PostgreSQL database name |
| `POSTGRES_USER` | PostgreSQL user |
| `POSTGRES_PASSWORD` | PostgreSQL password |
| `JWT_KEY` | JWT signing key, minimum 32 characters |
| `NEXT_PUBLIC_API_BASE_URL` | Public API base URL used by Next.js |

## Assumptions

- Each student belongs to one class or course at a time.
- A teacher may be assigned to many class and subject combinations.
- Assignment answers are text-based. File upload is outside the required scope.
- One student can have one current submission per assignment.
- A reviewed submission is locked against student changes.
- Admins can view all data but do not grade submissions.
- Dates are stored in UTC and displayed in the browser's local time.
- Deleting a user is implemented as deactivation to preserve academic records.

## Known limitations

- No password reset or email verification flow.
- No file attachments.
- No real-time notifications.
- No refresh-token rotation.
- Admin class and subject pages focus on creation and safe deletion, not inline editing.
- The demo uses local browser storage for the access token. A production deployment should use a stronger token strategy and HTTPS.

## Design decisions

- PostgreSQL was selected because the domain contains clear relational constraints and many-to-many relationships.
- `TeachingAssignment` resolves the many-to-many relationship among teachers, classes, and subjects.
- `Submission` has a unique `(AssignmentId, StudentId)` constraint to enforce one submission per student.
- Service-level workflow validation keeps deadline, ownership, resubmission, and marks rules independently testable.
- API DTOs prevent password hashes and internal navigation properties from being exposed.
- Docker Compose provides the easiest evaluator setup while manual instructions remain available.

## Submission checklist

See `docs/SUBMISSION_CHECKLIST.md` before uploading the repository and submitting the assessment.

## License

This recruitment assignment code is provided for evaluation and portfolio demonstration. See `LICENSE`.
