using NumberToWords.Domain;
using NumberToWords.Domain.Interfaces;
using System.Globalization;

namespace NumberToWords.Infrastructure.Validators;

public class CurrencyInputValidator : IInputValidator
{
    private const decimal MaxValue = 999999.99m;
    private const decimal MinValue = 0m;
    
    public ValidationResult Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return ValidationResult.Failure("Input cannot be empty");
        }
        
        // Try to parse as decimal (only allow decimal point, no thousands separators)
        if (!decimal.TryParse(input, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, 
            CultureInfo.InvariantCulture, out var value))
        {
            return ValidationResult.Failure("Invalid number format");
        }
        
        // Check for more than 2 decimal places by checking input string
        if (input.Contains('.'))
        {
            var decimalPart = input.Split('.')[1];
            if (decimalPart.Length > 2)
            {
                return ValidationResult.Failure("Currency values cannot have more than 2 decimal places");
            }
        }
        
        // Check range
        if (value < MinValue)
        {
            return ValidationResult.Failure("Negative numbers are not currently supported");
        }
        
        if (value > MaxValue)
        {
            return ValidationResult.Failure($"Value must be between {MinValue} and {MaxValue}");
        }
        
        return ValidationResult.Success(value);
    }
}