var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(isReadOnly: false)
    .WithDbGate();

var pgdb = postgres.AddDatabase("propulse-db");

// Add database migrations project
var dbMigrator = builder.AddProject<Projects.ProPulse_Database>("database-migrator")
    .WithReference(pgdb, "DefaultConnection")
    .WaitFor(pgdb);

// Add Azurite (Azure Storage Emulator)
var azurite = builder.AddAzureStorage("storage")
    .RunAsEmulator()
    .AddBlobs("media");

// Add the API project
var api = builder.AddProject<Projects.ProPulse_Api>("propulse-api")
    .WithReference(pgdb, "DefaultConnection").WaitFor(pgdb)
    .WithReference(azurite, "AzureStorage").WaitFor(azurite)
    .WaitForCompletion(dbMigrator)
    .WithExternalHttpEndpoints();

// Add the Web project
var web = builder.AddProject<Projects.ProPulse_Web>("web")
    .WithReference(api).WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
