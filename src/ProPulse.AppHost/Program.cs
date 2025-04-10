var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(isReadOnly: false)
    .WithDbGate();

var pgdb = postgres.AddDatabase("propulse-db");

// Add Azurite (Azure Storage Emulator)
var azurite = builder.AddAzureStorage("storage")
    .RunAsEmulator()
    .AddBlobs("media");

// Add the API project
var api = builder.AddProject<Projects.ProPulse_Api>("api")
    .WithReference(pgdb).WaitFor(pgdb)
    .WithReference(azurite).WaitFor(azurite)
    .WithExternalHttpEndpoints();

// Add the Web project
var web = builder.AddProject<Projects.ProPulse_Web>("web")
    .WithReference(api).WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
