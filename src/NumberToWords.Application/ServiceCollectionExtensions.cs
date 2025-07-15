using Microsoft.Extensions.DependencyInjection;
using NumberToWords.Application.Interfaces;
using NumberToWords.Application.Services;

namespace NumberToWords.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<INumberConversionService, NumberConversionService>();
        
        return services;
    }
}