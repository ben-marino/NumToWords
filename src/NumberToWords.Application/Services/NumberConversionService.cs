using NumberToWords.Application.Interfaces;
using NumberToWords.Domain;
using NumberToWords.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace NumberToWords.Application.Services;

public class NumberConversionService : INumberConversionService
{
    private readonly INumberToWordsConverter _converter;
    private readonly IInputValidator _validator;
    private readonly ILogger<NumberConversionService> _logger;
    
    public NumberConversionService(
        INumberToWordsConverter converter,
        IInputValidator validator,
        ILogger<NumberConversionService> logger)
    {
        _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<ConversionResult> ConvertAsync(string input)
    {
        return await ConvertAsync(input, new ConversionOptions());
    }
    
    public async Task<ConversionResult> ConvertAsync(string input, ConversionOptions options)
    {
        try
        {
            _logger.LogInformation("Starting conversion for input: {Input}", input);
            
            // Validate input
            var validationResult = _validator.Validate(input);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for input: {Input}. Error: {Error}", 
                    input, validationResult.ErrorMessage);
                return ConversionResult.Failure(validationResult.ErrorMessage!);
            }
            
            // Convert to words
            var result = await Task.Run(() => 
                _converter.Convert(validationResult.ParsedValue!.Value, options));
            
            _logger.LogInformation("Successfully converted {Value} to: {Result}", 
                validationResult.ParsedValue, result);
                
            return ConversionResult.Success(result, validationResult.ParsedValue!.Value);
        }
        catch (NotImplementedException ex)
        {
            _logger.LogWarning(ex, "Feature not implemented for input: {Input}", input);
            return ConversionResult.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error converting input: {Input}", input);
            return ConversionResult.Failure("An unexpected error occurred during conversion");
        }
    }
}