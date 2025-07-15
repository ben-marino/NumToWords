using Microsoft.Extensions.DependencyInjection;
using NumberToWords.Domain.Interfaces;
using NumberToWords.Infrastructure.Converters;
using NumberToWords.Infrastructure.Validators;

namespace NumberToWords.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<INumberToWordsConverter, CurrencyConverter>();
        services.AddSingleton<IInputValidator, CurrencyInputValidator>();
        
        return services;
    }
}