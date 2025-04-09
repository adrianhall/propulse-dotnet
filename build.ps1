# Build script for the ProPulse solution

Write-Host "Restoring dependencies..."
dotnet restore

Write-Host "Building solution..."
dotnet build --no-restore

Write-Host "Running tests with code coverage..."
# Create the coverage results directory if it doesn't exist
$coverageDir = "coverage-results"
if (-not (Test-Path $coverageDir)) {
    New-Item -ItemType Directory -Path $coverageDir | Out-Null
}

# Run tests with the XPlat Code Coverage collector
dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"

# Find all the coverage files
$coverageFiles = Get-ChildItem -Path test -Filter "coverage.cobertura.xml" -Recurse
if ($coverageFiles.Count -eq 0) {
    Write-Host "No coverage files found after test run." -ForegroundColor Red
    exit 1
}

Write-Host "Found $($coverageFiles.Count) coverage files." -ForegroundColor Green

# Check if ReportGenerator is installed, and install it if not
$reportGenTool = "dotnet-reportgenerator-globaltool"
if (-not (dotnet tool list -g | Select-String $reportGenTool)) {
    Write-Host "Installing ReportGenerator tool..." -ForegroundColor Cyan
    dotnet tool install -g $reportGenTool
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to install ReportGenerator tool. Code coverage reports will not be generated." -ForegroundColor Red
        exit $LASTEXITCODE
    }
}

# Create paths for the reports
Write-Host "Copying coverage files to $coverageDir..." -ForegroundColor Green
foreach ($file in $coverageFiles) {
    $testProject = Split-Path (Split-Path (Split-Path $file.FullName -Parent) -Parent) -Leaf
    $targetFile = Join-Path $coverageDir "coverage.$testProject.xml"
    Copy-Item $file.FullName $targetFile -Force
    Write-Host "Copied $($file.FullName) to $targetFile" -ForegroundColor Cyan
}

# Get all the coverage files in the coverage directory
$reports = Get-ChildItem -Path $coverageDir -Filter "coverage.*.xml" | ForEach-Object { $_.FullName }
$reportsJoined = $reports -join ";"

# Generate a combined report
Write-Host "Generating combined coverage report..."
reportgenerator `
    -reports:"$reportsJoined" `
    -targetdir:"$coverageDir/report" `
    -reporttypes:"Html;TextSummary" `
    -title:"ProPulse Coverage Report"

# Define which source projects we want to check coverage for
$projectsForCoverage = (Get-ChildItem -Path src -Exclude "*AppHost*", "*ServiceDefaults*", "*.Tests*" -Directory).Name | ForEach-Object { "src/$_" }

# Generate per-project coverage reports
foreach ($project in $projectsForCoverage) {
    $projectName = Split-Path $project -Leaf
    Write-Host "`nGenerating coverage report for $projectName..." -ForegroundColor Cyan
    
    # Generate HTML reports using ReportGenerator
    reportgenerator `
        -reports:"$reportsJoined" `
        -targetdir:"$coverageDir/report-$projectName" `
        -reporttypes:Html `
        -filters:"+$project/*" `
        -title:"$projectName Coverage Report"
}

Write-Host "`nCode coverage reports generated in $coverageDir/report" -ForegroundColor Green
Write-Host "Build completed successfully!" -ForegroundColor Green

# Display summary if it exists
$summaryFile = "$coverageDir/report/Summary.txt"
if (Test-Path $summaryFile) {
    Write-Host "`nCoverage Summary:" -ForegroundColor Green
    Get-Content $summaryFile | ForEach-Object { Write-Host $_ }
}