using NumberToWords.Application;
using NumberToWords.Application.Interfaces;
using NumberToWords.Domain;
using NumberToWords.Infrastructure;
using NumberToWords.Web.Models;
using System.Diagnostics;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/numbertowords-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Number to Words API", Version = "v1" });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add application services
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// Add Razor Pages for the web interface
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Number to Words API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseStaticFiles();
app.UseRouting();

// Map API endpoints
app.MapPost("/api/convert", async (ConvertRequest request, INumberConversionService service) =>
{
    var stopwatch = Stopwatch.StartNew();
    
    try
    {
        ConversionResult result;
        
        if (request.Options != null)
        {
            var options = new ConversionOptions
            {
                CurrencyName = request.Options.CurrencyName ?? "DOLLARS",
                SubCurrencyName = request.Options.SubCurrencyName ?? "CENTS",
                UseAndConnector = request.Options.UseAndConnector ?? true,
                Style = Enum.TryParse<CapitalizationStyle>(request.Options.Style, out var style) 
                    ? style 
                    : CapitalizationStyle.AllUpper
            };
            result = await service.ConvertAsync(request.Input, options);
        }
        else
        {
            result = await service.ConvertAsync(request.Input);
        }
        
        stopwatch.Stop();
        
        var response = new ConvertResponse
        {
            IsSuccess = result.IsSuccess,
            Result = result.Result,
            ErrorMessage = result.ErrorMessage,
            OriginalValue = result.OriginalValue,
            ProcessingTimeMs = stopwatch.ElapsedMilliseconds
        };
        
        return result.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Unexpected error in convert endpoint");
        stopwatch.Stop();
        
        var errorResponse = new ConvertResponse
        {
            IsSuccess = false,
            ErrorMessage = "An unexpected error occurred",
            ProcessingTimeMs = stopwatch.ElapsedMilliseconds
        };
        
        return Results.StatusCode(500);
    }
})
.WithName("ConvertNumberToWords")
.WithOpenApi()
.WithSummary("Converts a number to its word representation")
.WithDescription("Converts decimal numbers (0-999,999.99) to their currency word representation");

// Map Razor Pages
app.MapRazorPages();
app.MapFallbackToPage("/Index");

app.Run();
