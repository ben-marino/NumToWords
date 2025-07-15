using FluentAssertions;
using NumberToWords.Domain;

namespace NumberToWords.Tests.Unit;

public class MoneyAmountTests
{
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(1.50, 1, 50)]
    [InlineData(123.45, 123, 45)]
    [InlineData(999.99, 999, 99)]
    [InlineData(1000, 1000, 0)]
    [InlineData(1000.01, 1000, 1)]
    public void Constructor_ValidPositiveValues_SetsPropertiesCorrectly(decimal value, int expectedDollars, int expectedCents)
    {
        // Act
        var moneyAmount = new MoneyAmount(value);

        // Assert
        moneyAmount.Value.Should().Be(value);
        moneyAmount.DollarPart.Should().Be(expectedDollars);
        moneyAmount.CentPart.Should().Be(expectedCents);
    }

    [Theory]
    [InlineData(-1, 1, 0)]
    [InlineData(-123.45, 123, 45)]
    [InlineData(-999.99, 999, 99)]
    public void Constructor_NegativeValues_UsesAbsoluteValueForParts(decimal value, int expectedDollars, int expectedCents)
    {
        // Act
        var moneyAmount = new MoneyAmount(value);

        // Assert
        moneyAmount.Value.Should().Be(value);
        moneyAmount.DollarPart.Should().Be(expectedDollars);
        moneyAmount.CentPart.Should().Be(expectedCents);
    }

    [Theory]
    [InlineData(0.001, 0, 0)]   // Rounds down to 0 cents
    [InlineData(0.004, 0, 0)]   // Rounds down to 0 cents
    [InlineData(0.005, 0, 0)]   // Rounds down to 0 cents (banker's rounding)
    [InlineData(0.009, 0, 1)]   // Rounds up to 1 cent
    [InlineData(123.994, 123, 99)]  // Rounds down to 99 cents
    [InlineData(123.995, 124, 0)]   // Rounds to 100 cents, then carries over to dollars
    public void Constructor_FractionalCents_RoundsCorrectly(decimal value, int expectedDollars, int expectedCents)
    {
        // Act
        var moneyAmount = new MoneyAmount(value);

        // Assert
        moneyAmount.DollarPart.Should().Be(expectedDollars);
        moneyAmount.CentPart.Should().Be(expectedCents);
    }

    [Fact]
    public void Constructor_LargeValue_HandlesCorrectly()
    {
        // Arrange
        var largeValue = 999999.99m;

        // Act
        var moneyAmount = new MoneyAmount(largeValue);

        // Assert
        moneyAmount.Value.Should().Be(largeValue);
        moneyAmount.DollarPart.Should().Be(999999);
        moneyAmount.CentPart.Should().Be(99);
    }

    [Fact]
    public void Constructor_ExactDollarAmount_HasZeroCents()
    {
        // Arrange
        var exactDollars = 42m;

        // Act
        var moneyAmount = new MoneyAmount(exactDollars);

        // Assert
        moneyAmount.Value.Should().Be(exactDollars);
        moneyAmount.DollarPart.Should().Be(42);
        moneyAmount.CentPart.Should().Be(0);
    }

    [Fact]
    public void Constructor_OnlyFractionalCents_HasZeroDollars()
    {
        // Arrange
        var centsOnly = 0.75m;

        // Act
        var moneyAmount = new MoneyAmount(centsOnly);

        // Assert
        moneyAmount.Value.Should().Be(centsOnly);
        moneyAmount.DollarPart.Should().Be(0);
        moneyAmount.CentPart.Should().Be(75);
    }

    [Theory]
    [InlineData(1.01)]
    [InlineData(2.02)]
    [InlineData(10.10)]
    [InlineData(100.01)]
    public void Constructor_SingleDigitCents_PreservesValue(decimal value)
    {
        // Act
        var moneyAmount = new MoneyAmount(value);

        // Assert
        moneyAmount.Value.Should().Be(value);
        var expectedCents = (int)Math.Round((value - Math.Floor(value)) * 100);
        moneyAmount.CentPart.Should().Be(expectedCents);
    }

    [Fact]
    public void Constructor_MinimumValue_HandlesCorrectly()
    {
        // Arrange
        var minimumValue = 0.01m;

        // Act
        var moneyAmount = new MoneyAmount(minimumValue);

        // Assert
        moneyAmount.Value.Should().Be(minimumValue);
        moneyAmount.DollarPart.Should().Be(0);
        moneyAmount.CentPart.Should().Be(1);
    }

    [Fact]
    public void MoneyAmount_IsValueType_BehavesAsExpected()
    {
        // Arrange
        var amount1 = new MoneyAmount(123.45m);
        var amount2 = new MoneyAmount(123.45m);
        var amount3 = new MoneyAmount(100.00m);

        // Assert
        amount1.Equals(amount2).Should().BeTrue();
        amount1.Equals(amount3).Should().BeFalse();
        amount1.Value.Should().Be(amount2.Value);
        amount1.Value.Should().NotBe(amount3.Value);
    }
}