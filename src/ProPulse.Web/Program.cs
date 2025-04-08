var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
    {
        // Use secure random number generator
        int GetSecureRandomNumber(int minValue, int maxValue)
        {
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var randomValue = BitConverter.ToInt32(bytes, 0);
            return Math.Abs(randomValue % (maxValue - minValue)) + minValue;
        }

        int randomTemp = GetSecureRandomNumber(-20, 55);
        int randomSummaryIndex = GetSecureRandomNumber(0, summaries.Length);

        return new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            randomTemp,
            summaries[randomSummaryIndex]
        );
    })
    .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

/// <summary>
/// Represents a weather forecast with date, temperature, and summary information.
/// </summary>
/// <param name="Date">The date of the forecast</param>
/// <param name="TemperatureC">The temperature in Celsius</param>
/// <param name="Summary">A text summary of the weather conditions</param>
sealed record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    /// Gets the temperature in Fahrenheit, converted from Celsius.
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
