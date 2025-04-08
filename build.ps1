# Build script for the ProPulse solution

Write-Host "Restoring dependencies..."
dotnet restore

Write-Host "Building solution..."
dotnet build --no-restore

Write-Host "Running tests..."
dotnet test --no-build --verbosity normal

Write-Host "Build completed successfully!"
