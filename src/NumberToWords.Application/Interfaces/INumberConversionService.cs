using NumberToWords.Domain;

namespace NumberToWords.Application.Interfaces;

public interface INumberConversionService
{
    Task<ConversionResult> ConvertAsync(string input);
    Task<ConversionResult> ConvertAsync(string input, ConversionOptions options);
}