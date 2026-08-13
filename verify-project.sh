#!/usr/bin/env sh
set -eu

echo "[1/4] Restoring and building the backend"
dotnet restore AssignmentSubmission.sln
dotnet build AssignmentSubmission.sln --configuration Release --no-restore

echo "[2/4] Running backend tests"
dotnet test backend/AssignmentManagement.Tests/AssignmentManagement.Tests.csproj --configuration Release --no-build

echo "[3/4] Installing and building the frontend"
cd frontend
npm install
npm run build
cd ..

echo "[4/4] Validating Docker Compose"
docker compose config >/dev/null

echo "Verification completed successfully."
