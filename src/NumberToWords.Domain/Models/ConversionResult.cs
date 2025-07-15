namespace NumberToWords.Domain;

public class ConversionResult
{
    public bool IsSuccess { get; init; }
    public string? Result { get; init; }
    public string? ErrorMessage { get; init; }
    public decimal? OriginalValue { get; init; }

    public static ConversionResult Success(string result, decimal originalValue) =>
        new() { IsSuccess = true, Result = result, OriginalValue = originalValue };

    public static ConversionResult Failure(string errorMessage) =>
        new() { IsSuccess = false, ErrorMessage = errorMessage };
}