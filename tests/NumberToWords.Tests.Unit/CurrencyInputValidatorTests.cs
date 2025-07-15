using FluentAssertions;
using NumberToWords.Infrastructure.Validators;

namespace NumberToWords.Tests.Unit;

public class CurrencyInputValidatorTests
{
    private readonly CurrencyInputValidator _validator;

    public CurrencyInputValidatorTests()
    {
        _validator = new CurrencyInputValidator();
    }

    [Theory]
    [InlineData("0")]
    [InlineData("0.00")]
    [InlineData("1")]
    [InlineData("1.00")]
    [InlineData("123.45")]
    [InlineData("999999.99")]
    [InlineData("42")]
    [InlineData("1000")]
    [InlineData("0.01")]
    [InlineData("100.50")]
    public void Validate_ValidInput_ReturnsSuccessResult(string input)
    {
        // Act
        var result = _validator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.ParsedValue.Should().NotBeNull();
        result.ParsedValue.Should().BeGreaterThanOrEqualTo(0);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_EmptyOrWhitespaceInput_ReturnsFailureResult(string input)
    {
        // Act
        var result = _validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Input cannot be empty");
        result.ParsedValue.Should().BeNull();
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("12.34.56")]
    [InlineData("12a.34")]
    [InlineData("$123.45")]
    [InlineData("123,456.78")]
    [InlineData("not a number")]
    public void Validate_InvalidNumberFormat_ReturnsFailureResult(string input)
    {
        // Act
        var result = _validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid number format");
        result.ParsedValue.Should().BeNull();
    }

    [Theory]
    [InlineData("123.456")]
    [InlineData("1.234")]
    [InlineData("0.001")]
    [InlineData("42.1234")]
    [InlineData("999999.991")]
    public void Validate_TooManyDecimalPlaces_ReturnsFailureResult(string input)
    {
        // Act
        var result = _validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Currency values cannot have more than 2 decimal places");
        result.ParsedValue.Should().BeNull();
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("-123.45")]
    [InlineData("-0.01")]
    public void Validate_NegativeNumbers_ReturnsFailureResult(string input)
    {
        // Act
        var result = _validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Negative numbers are not currently supported");
        result.ParsedValue.Should().BeNull();
    }

    [Theory]
    [InlineData("1000000")]
    [InlineData("1000000.00")]
    [InlineData("1234567.89")]
    public void Validate_ValueAboveMaximum_ReturnsFailureResult(string input)
    {
        // Act
        var result = _validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Value must be between 0 and 999999.99");
        result.ParsedValue.Should().BeNull();
    }

    [Theory]
    [InlineData("0", 0)]
    [InlineData("1", 1)]
    [InlineData("123.45", 123.45)]
    [InlineData("999999.99", 999999.99)]
    [InlineData("0.01", 0.01)]
    [InlineData("1000.00", 1000.00)]
    public void Validate_ValidInput_ParsesValueCorrectly(string input, decimal expectedValue)
    {
        // Act
        var result = _validator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
        result.ParsedValue.Should().Be(expectedValue);
    }

    [Fact]
    public void Validate_BoundaryValueMaximum_ReturnsSuccessResult()
    {
        // Arrange
        var input = "999999.99";

        // Act
        var result = _validator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
        result.ParsedValue.Should().Be(999999.99m);
    }

    [Fact]
    public void Validate_BoundaryValueMinimum_ReturnsSuccessResult()
    {
        // Arrange
        var input = "0";

        // Act
        var result = _validator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
        result.ParsedValue.Should().Be(0m);
    }
}