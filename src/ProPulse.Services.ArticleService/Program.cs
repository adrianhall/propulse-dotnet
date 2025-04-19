using ProPulse.DataModel;
using ProPulse.Services.ArticleService.Mapping;
using Scalar.AspNetCore;
using Serilog;

// ==================================================================================
// Service Configuration.
// ==================================================================================
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// ==================================================================================
// Services and Dependency Injection
// ==================================================================================

// Add Serilog logging configuration
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

// Register the data model (DbContext, etc.)
builder.Services.AddDataModel(builder.Configuration);

// Register AutoMapper for translating between entity and DTO.
builder.Services.AddAutoMapper(typeof(ArticleMappingProfile).Assembly);

// Generate OpenAPI Documents
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add MVC controllers for APIs
builder.Services.AddControllers();

// ==================================================================================
// Application Initialization
// ==================================================================================
var app = builder.Build();

// ==================================================================================
// HTTP Pipeline setup
// ==================================================================================

// Add Serilog request logging middleware
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

// ==================================================================================
// Endpoint mapping
// ==================================================================================

app.MapDefaultEndpoints();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// ==================================================================================
// Run the application
// ==================================================================================

app.Run();

// ==================================================================================
// Partial class for testing
// ==================================================================================
public partial class Program
{
    // This partial class is used for testing purposes. It allows the test project to access the Program class.
    // The test project can use this to create a WebApplicationFactory for integration tests.
}