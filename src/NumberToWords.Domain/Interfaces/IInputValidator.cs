namespace NumberToWords.Domain.Interfaces;

public interface IInputValidator
{
    ValidationResult Validate(string input);
}