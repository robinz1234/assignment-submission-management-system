# Step-by-step operating guide

## Part A, start the full project on Windows

1. Install and open Docker Desktop.
2. Extract the ZIP file.
3. Open the extracted `Assignment_Submission_Management_System` folder.
4. Click the folder address bar, type `powershell`, and press Enter.
5. Run:

```powershell
Copy-Item .env.example .env
docker compose up --build
```

6. Wait until the terminal shows that the web, API, and database containers are running.
7. Open `http://localhost:3000`.
8. Keep the terminal open while using the project.

## Part B, test the Admin workflow

1. Login with `admin@school.test` and `Admin123!`.
2. Open **Users**.
3. Create a teacher or student. A student must have a class.
4. Open **Academic setup**.
5. Create a class and a subject.
6. Select a teacher, class, and subject, then click **Assign**.
7. Open **Assignments** and **Submissions** to view records across the system.
8. Open **Settings** to edit the school name or resubmission default.

## Part C, test the Teacher workflow

1. Logout and login with `teacher@school.test` and `Teacher123!`.
2. Open **Assignments**.
3. Click **New assignment**.
4. Select the assigned class and subject.
5. Enter title, description, deadline, maximum marks, and resubmission choice.
6. Select **Draft** to save privately or **Published** to make it visible to students.
7. Open an assignment to edit, publish, or delete it.
8. Open **Submissions**, then select the related assignment.
9. Enter marks and feedback, choose a status, and save the review.

## Part D, test the Student workflow

1. Logout and login with `student@school.test` and `Student123!`.
2. Open **Assignments**.
3. Open a published assignment.
4. Enter an answer and submit it before the deadline.
5. Update the answer while resubmission is allowed and the work is not reviewed.
6. Open **My submissions** to see status, marks, and feedback.

## Part E, view and test the API

1. Open `http://localhost:5050/swagger`.
2. Use `POST /api/auth/login` with a demo account.
3. Copy the returned token.
4. Click **Authorize** in Swagger and enter the token.
5. Test endpoints allowed for that role.
6. Confirm that restricted endpoints return `403 Forbidden` for the wrong role.

## Part F, run automated tests

This step needs the .NET 8 SDK installed on the computer.

```powershell
dotnet test AssignmentSubmission.sln
```

## Part G, stop or reset

Stop while keeping data:

```powershell
docker compose down
```

Reset all database data:

```powershell
.\reset-docker.ps1
```

## Common fixes

### Port already in use

Stop software using ports `3000`, `5050`, or `5432`, or stop an older Docker project:

```powershell
docker compose down
```

### Frontend cannot reach API

Check that `http://localhost:5050/health` returns a healthy response. Also confirm `.env` contains:

```text
NEXT_PUBLIC_API_BASE_URL=http://localhost:5050/api
```

### Database or migration error

Reset the Docker volume:

```powershell
docker compose down -v
docker compose up --build
```

### Changes do not appear

Rebuild containers:

```powershell
docker compose up --build --force-recreate
```
