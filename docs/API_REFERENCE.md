# API reference

Base URL: `http://localhost:5050/api`

Protected endpoints require:

```http
Authorization: Bearer <JWT_TOKEN>
```

## Authentication

| Method | Endpoint | Access | Purpose |
|---|---|---|---|
| POST | `/auth/login` | Public | Login and receive a JWT |
| GET | `/auth/me` | Authenticated | Read the current user profile |

## Dashboard

| Method | Endpoint | Access | Purpose |
|---|---|---|---|
| GET | `/dashboard` | All roles | Role-specific metrics and recent activity |

## Assignments

| Method | Endpoint | Access | Purpose |
|---|---|---|---|
| GET | `/assignments` | All roles | Paginated and filtered assignments within role scope |
| GET | `/assignments/{id}` | All roles | Assignment detail within role scope |
| POST | `/assignments` | Teacher | Create an assignment within an assigned teaching scope |
| PUT | `/assignments/{id}` | Teacher | Update own assignment |
| DELETE | `/assignments/{id}` | Teacher | Delete own assignment when no submissions exist |
| POST | `/assignments/{id}/publish` | Teacher | Publish own assignment |

Common list query parameters: `page`, `pageSize`, `search`, `status`, `classId`, and `subjectId`.

## Submissions

| Method | Endpoint | Access | Purpose |
|---|---|---|---|
| GET | `/submissions` | Teacher, Admin | Paginated submissions within role scope |
| GET | `/submissions/my` | Student | List the current student's submissions |
| GET | `/submissions/{id}` | All roles | Submission detail within role scope |
| POST | `/submissions/assignments/{assignmentId}` | Student | Create an answer before the deadline |
| PUT | `/submissions/{id}` | Student | Update own answer when allowed |
| PUT | `/submissions/{id}/review` | Teacher | Set status, marks, and feedback |

## Reference data

| Method | Endpoint | Access | Purpose |
|---|---|---|---|
| GET | `/reference/classes` | Authenticated | Class options |
| GET | `/reference/subjects` | Authenticated | Subject options scoped for teachers |
| GET | `/reference/teacher-scopes` | Teacher | Current teacher's assigned class and subject scopes |

## Admin, users

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/admin/users` | Search, filter, and paginate users |
| GET | `/admin/users/{id}` | Read one user |
| POST | `/admin/users` | Create a user |
| PUT | `/admin/users/{id}` | Update a user or password |
| DELETE | `/admin/users/{id}` | Deactivate a user |

## Admin, academic setup

| Method | Endpoint | Purpose |
|---|---|---|
| GET, POST | `/admin/classes` | List and create classes |
| DELETE | `/admin/classes/{id}` | Delete an unused class |
| GET, POST | `/admin/subjects` | List and create subjects |
| DELETE | `/admin/subjects/{id}` | Delete an unused subject |
| GET, POST | `/admin/teaching-assignments` | List and create teacher scopes |
| DELETE | `/admin/teaching-assignments/{id}` | Remove a teacher scope |

## Admin, settings

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/admin/settings` | List application settings |
| PUT | `/admin/settings/{id}` | Update a setting value and description |

## Error response

The API middleware returns a Problem Details response. Example:

```json
{
  "type": "https://httpstatuses.com/400",
  "title": "The assignment deadline has passed.",
  "status": 400,
  "traceId": "..."
}
```
