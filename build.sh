#!/bin/bash

# Exit on error
set -e

echo "Restoring dependencies..."
dotnet restore

echo "Building solution..."
dotnet build --no-restore

echo "Running tests with code coverage..."
# Create the coverage results directory if it doesn't exist
coverage_dir="coverage-results"

# Remove the coverage results directory if it exists
if [ -d "$coverage_dir" ]; then
    rm -rf "$coverage_dir"
    echo "Removed existing coverage directory."
fi

mkdir -p "$coverage_dir"

# Run tests with the XPlat Code Coverage collector
dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"

# Find all the coverage files
coverage_files=$(find test -name "coverage.cobertura.xml" -type f)
if [ -z "$coverage_files" ]; then
    echo "No coverage files found after test run."
    exit 1
fi

echo "Found coverage files."

# Check if ReportGenerator is installed, and install it if not
if ! dotnet tool list -g | grep "dotnet-reportgenerator-globaltool" > /dev/null; then
    echo "Installing ReportGenerator tool..."
    dotnet tool install -g dotnet-reportgenerator-globaltool
    if [ $? -ne 0 ]; then
        echo "Failed to install ReportGenerator tool. Code coverage reports will not be generated."
        exit 1
    fi
fi

# Copy coverage files to coverage directory
echo "Copying coverage files to $coverage_dir..."
for file in $coverage_files; do
    test_project=$(basename $(dirname $(dirname $(dirname $file))))
    target_file="$coverage_dir/coverage.$test_project.xml"
    cp "$file" "$target_file"
    echo "Copied $file to $target_file"
done

# Get all the coverage files in the coverage directory
reports=$(find "$coverage_dir" -name "coverage.*.xml" -type f | tr '\n' ';')

# Generate a combined report
echo "Generating combined coverage report..."
reportgenerator \
    -reports:"$reports" \
    -targetdir:"$coverage_dir/report" \
    -reporttypes:"Html;TextSummary" \
    -title:"ProPulse Coverage Report"

# Define which source projects we want to check coverage for
projects_for_coverage=$(find src -type d -not -path "*/\.*" -not -path "*/bin/*" -not -path "*/obj/*" \
    -not -path "*AppHost*" -not -path "*ServiceDefaults*" -not -path "*.Tests*" -mindepth 1 -maxdepth 1)

# Generate per-project coverage reports
for project in $projects_for_coverage; do
    project_name=$(basename "$project")
    echo -e "\nGenerating coverage report for $project_name..."
    
    # Generate HTML reports using ReportGenerator
    reportgenerator \
        -reports:"$reports" \
        -targetdir:"$coverage_dir/report-$project_name" \
        -reporttypes:Html \
        -filters:"+$project/*" \
        -title:"$project_name Coverage Report"
done

echo -e "\nCode coverage reports generated in $coverage_dir/report"
echo "Build completed successfully!"

# Display summary if it exists
summary_file="$coverage_dir/report/Summary.txt"
if [ -f "$summary_file" ]; then
    echo -e "\nCoverage Summary:"
    cat "$summary_file"
fi