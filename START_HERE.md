# Start here

## Fastest way to run on Windows

1. Install Docker Desktop.
2. Start Docker Desktop.
3. Extract the project ZIP.
4. Open PowerShell inside the extracted project folder.
5. Run:

```powershell
.\start-docker.ps1
```

6. Wait for the three services to start.
7. Open `http://localhost:3000`.
8. Login with one of these accounts:

| Role | Email | Password |
|---|---|---|
| Admin | `admin@school.test` | `Admin123!` |
| Teacher | `teacher@school.test` | `Teacher123!` |
| Student | `student@school.test` | `Student123!` |

## Important links

- Application: `http://localhost:3000`
- Swagger: `http://localhost:5050/swagger`
- Health check: `http://localhost:5050/health`

## Stop

Press `Ctrl + C`, then run:

```powershell
docker compose down
```

## Full instructions

Read `README.md` and `docs/OPERATING_GUIDE.md`.
