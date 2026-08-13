# GitHub and submission guide

## 1. Create the repository

1. Sign in to GitHub.
2. Create a new repository, for example `assignment-submission-management-system`.
3. Keep it public or make sure the evaluator can access it.
4. Do not initialize it with another README because this project already contains one.

## 2. Upload with Git

Open PowerShell in the project root:

```powershell
git init
git add .
git commit -m "Complete assignment and submission management system"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/assignment-submission-management-system.git
git push -u origin main
```

## 3. Verify before submission

1. Open the repository in a private browser window.
2. Confirm the source code is visible.
3. Confirm `.env` is not committed.
4. Confirm `.env.example` is committed.
5. Confirm the README displays correctly.
6. Confirm the `backend`, `frontend`, `database`, and `docs` folders are present.
7. Confirm GitHub Actions completes successfully.

## 4. Optional live deployment

The assessment does not require a live URL. Docker-based local setup is complete. A live frontend and API may be added later if hosting is available.

## 5. Submit

Use the project submission link shown in the assignment PDF. Provide the accessible Git repository URL and any optional live links.
