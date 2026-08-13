$ErrorActionPreference = "Stop"

Write-Host "[1/4] Restoring and building the backend"
dotnet restore AssignmentSubmission.sln
dotnet build AssignmentSubmission.sln --configuration Release --no-restore

Write-Host "[2/4] Running backend tests"
dotnet test backend/AssignmentManagement.Tests/AssignmentManagement.Tests.csproj --configuration Release --no-build

Write-Host "[3/4] Installing and building the frontend"
Push-Location frontend
try {
    npm install
    npm run build
}
finally {
    Pop-Location
}

Write-Host "[4/4] Validating Docker Compose"
docker compose config | Out-Null

Write-Host "Verification completed successfully."
