#!/bin/bash

# Exit on error
set -e

echo "Restoring dependencies..."
dotnet restore

echo "Building solution..."
dotnet build --no-restore

echo "Running tests..."
dotnet test --no-build --verbosity normal

echo "Build completed successfully!"
