// ---------------------------------------------------------------
// ProPulse Aspire Orchestration for Development
// ---------------------------------------------------------------

var builder = DistributedApplication.CreateBuilder(args);

// ---------------------------------------------------------------
// Database Services
// ---------------------------------------------------------------

var dbService = builder.AddPostgres("db-service").WithDbGate();

var database = dbService.AddDatabase("propulse-db");

// ---------------------------------------------------------------
// Storage Services
// ---------------------------------------------------------------

var azurite = builder.AddAzureStorage("azurite").RunAsEmulator();

var blobStorage = azurite.AddBlobs("blob-storage");

// ---------------------------------------------------------------
// Database Migration Services
// ---------------------------------------------------------------

var migrator = builder.AddProject<Projects.ProPulse_DataModel_Initializer>("db-migrations")
    .WithReference(database, "DefaultConnection")
    .WaitFor(database);

// ---------------------------------------------------------------
// API Services
// ---------------------------------------------------------------

// ---------------------------------------------------------------
// Web Frontend
// ---------------------------------------------------------------

// ---------------------------------------------------------------
// Run the orchestration
// ---------------------------------------------------------------

builder.Build().Run();
