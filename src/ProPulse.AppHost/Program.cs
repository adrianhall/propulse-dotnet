var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database resource
var postgres = builder.AddPostgres("db-service")
    .WithImageTag("17")
    .WithDbGate();

var database = postgres.AddDatabase("propulse");

// Add Redis cache
var redis = builder.AddRedis("redis-cache");

// Add the Web API project with references to PostgreSQL and Redis
var webApi = builder.AddProject<Projects.ProPulse_Web>("frontend")
    .WithReference(database)
    .WithReference(redis)
    .WaitFor(database)
    .WaitFor(redis);

builder.Build().Run();
