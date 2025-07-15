namespace NumberToWords.Domain;

public class ValidationResult
{
    public bool IsValid { get; init; }
    public string? ErrorMessage { get; init; }
    public decimal? ParsedValue { get; init; }

    public static ValidationResult Success(decimal value) =>
        new() { IsValid = true, ParsedValue = value };

    public static ValidationResult Failure(string errorMessage) =>
        new() { IsValid = false, ErrorMessage = errorMessage };
}