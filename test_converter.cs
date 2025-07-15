using NumberToWords.Domain;
using NumberToWords.Infrastructure.Converters;
using NumberToWords.Infrastructure.Validators;

// Quick test of the converter
var converter = new CurrencyConverter();
var validator = new CurrencyInputValidator();

Console.WriteLine("Testing Number to Words Converter:");
Console.WriteLine("===================================");

var testCases = new[]
{
    "123.45",
    "1000.00",
    "0.01",
    "999999.99",
    "42",
    "0"
};

foreach (var testCase in testCases)
{
    var validationResult = validator.Validate(testCase);
    if (validationResult.IsValid)
    {
        var result = converter.Convert(validationResult.ParsedValue!.Value);
        Console.WriteLine($"{testCase} => {result}");
    }
    else
    {
        Console.WriteLine($"{testCase} => ERROR: {validationResult.ErrorMessage}");
    }
}

Console.WriteLine("\nDone!");