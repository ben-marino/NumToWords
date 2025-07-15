using FluentAssertions;
using NumberToWords.Domain;
using NumberToWords.Infrastructure.Converters;

namespace NumberToWords.Tests.Unit;

public class CurrencyConverterTests
{
    private readonly CurrencyConverter _converter;

    public CurrencyConverterTests()
    {
        _converter = new CurrencyConverter();
    }

    [Theory]
    [InlineData(0, "ZERO DOLLARS AND ZERO CENTS")]
    [InlineData(1, "ONE DOLLAR AND ZERO CENTS")]
    [InlineData(2, "TWO DOLLARS AND ZERO CENTS")]
    [InlineData(10, "TEN DOLLARS AND ZERO CENTS")]
    [InlineData(11, "ELEVEN DOLLARS AND ZERO CENTS")]
    [InlineData(20, "TWENTY DOLLARS AND ZERO CENTS")]
    [InlineData(21, "TWENTY ONE DOLLARS AND ZERO CENTS")]
    [InlineData(100, "ONE HUNDRED DOLLARS AND ZERO CENTS")]
    [InlineData(101, "ONE HUNDRED ONE DOLLARS AND ZERO CENTS")]
    [InlineData(111, "ONE HUNDRED ELEVEN DOLLARS AND ZERO CENTS")]
    [InlineData(1000, "ONE THOUSAND DOLLARS AND ZERO CENTS")]
    [InlineData(1001, "ONE THOUSAND ONE DOLLARS AND ZERO CENTS")]
    [InlineData(1111, "ONE THOUSAND ONE HUNDRED ELEVEN DOLLARS AND ZERO CENTS")]
    public void Convert_WholeDollarAmounts_ReturnsCorrectWords(decimal amount, string expected)
    {
        // Act
        var result = _converter.Convert(amount);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0.01, "ZERO DOLLARS AND ONE CENT")]
    [InlineData(0.02, "ZERO DOLLARS AND TWO CENTS")]
    [InlineData(0.10, "ZERO DOLLARS AND TEN CENTS")]
    [InlineData(0.11, "ZERO DOLLARS AND ELEVEN CENTS")]
    [InlineData(0.20, "ZERO DOLLARS AND TWENTY CENTS")]
    [InlineData(0.21, "ZERO DOLLARS AND TWENTY ONE CENTS")]
    [InlineData(0.99, "ZERO DOLLARS AND NINETY NINE CENTS")]
    public void Convert_CentsOnly_ReturnsCorrectWords(decimal amount, string expected)
    {
        // Act
        var result = _converter.Convert(amount);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(123.45, "ONE HUNDRED TWENTY THREE DOLLARS AND FORTY FIVE CENTS")]
    [InlineData(1.01, "ONE DOLLAR AND ONE CENT")]
    [InlineData(12.34, "TWELVE DOLLARS AND THIRTY FOUR CENTS")]
    [InlineData(999.99, "NINE HUNDRED NINETY NINE DOLLARS AND NINETY NINE CENTS")]
    [InlineData(1000.01, "ONE THOUSAND DOLLARS AND ONE CENT")]
    [InlineData(12345.67, "TWELVE THOUSAND THREE HUNDRED FORTY FIVE DOLLARS AND SIXTY SEVEN CENTS")]
    public void Convert_DollarsAndCents_ReturnsCorrectWords(decimal amount, string expected)
    {
        // Act
        var result = _converter.Convert(amount);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Convert_MaxSupportedValue_ReturnsCorrectWords()
    {
        // Arrange
        var amount = 999999.99m;
        var expected = "NINE HUNDRED NINETY NINE THOUSAND NINE HUNDRED NINETY NINE DOLLARS AND NINETY NINE CENTS";

        // Act
        var result = _converter.Convert(amount);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(123.45, CapitalizationStyle.AllUpper, "ONE HUNDRED TWENTY THREE DOLLARS AND FORTY FIVE CENTS")]
    [InlineData(123.45, CapitalizationStyle.TitleCase, "One Hundred Twenty Three Dollars And Forty Five Cents")]
    [InlineData(123.45, CapitalizationStyle.SentenceCase, "One hundred twenty three dollars and forty five cents")]
    public void Convert_WithCapitalizationOptions_ReturnsCorrectFormat(decimal amount, CapitalizationStyle style, string expected)
    {
        // Arrange
        var options = new ConversionOptions { Style = style };

        // Act
        var result = _converter.Convert(amount, options);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Convert_WithCustomCurrencyNames_ReturnsCorrectWords()
    {
        // Arrange
        var amount = 123.45m;
        var options = new ConversionOptions 
        { 
            CurrencyName = "POUNDS", 
            SubCurrencyName = "PENCE" 
        };
        var expected = "ONE HUNDRED TWENTY THREE POUNDS AND FORTY FIVE PENCE";

        // Act
        var result = _converter.Convert(amount, options);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Convert_WithoutAndConnector_ReturnsCorrectWords()
    {
        // Arrange
        var amount = 123.45m;
        var options = new ConversionOptions { UseAndConnector = false };
        var expected = "ONE HUNDRED TWENTY THREE DOLLARS FORTY FIVE CENTS";

        // Act
        var result = _converter.Convert(amount, options);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Convert_NegativeNumber_ThrowsNotImplementedException()
    {
        // Arrange
        var amount = -123.45m;

        // Act & Assert
        _converter.Invoking(c => c.Convert(amount))
            .Should().Throw<NotImplementedException>()
            .WithMessage("Negative numbers not yet supported");
    }

    [Fact]
    public void Convert_ValueAboveMaximum_ThrowsNotImplementedException()
    {
        // Arrange
        var amount = 1000000m;

        // Act & Assert
        _converter.Invoking(c => c.Convert(amount))
            .Should().Throw<NotImplementedException>()
            .WithMessage("Numbers greater than 999,999.99 not yet supported");
    }

    [Theory]
    [InlineData(42.00, "FORTY TWO DOLLARS AND ZERO CENTS")]
    [InlineData(100.00, "ONE HUNDRED DOLLARS AND ZERO CENTS")]
    [InlineData(1000.00, "ONE THOUSAND DOLLARS AND ZERO CENTS")]
    public void Convert_ExactDollarAmounts_IncludesZeroCents(decimal amount, string expected)
    {
        // Act
        var result = _converter.Convert(amount);

        // Assert
        result.Should().Be(expected);
    }
}