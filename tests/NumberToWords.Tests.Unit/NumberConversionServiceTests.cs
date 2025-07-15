using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NumberToWords.Application.Services;
using NumberToWords.Domain;
using NumberToWords.Domain.Interfaces;

namespace NumberToWords.Tests.Unit;

public class NumberConversionServiceTests
{
    private readonly Mock<INumberToWordsConverter> _mockConverter;
    private readonly Mock<IInputValidator> _mockValidator;
    private readonly Mock<ILogger<NumberConversionService>> _mockLogger;
    private readonly NumberConversionService _service;

    public NumberConversionServiceTests()
    {
        _mockConverter = new Mock<INumberToWordsConverter>();
        _mockValidator = new Mock<IInputValidator>();
        _mockLogger = new Mock<ILogger<NumberConversionService>>();
        _service = new NumberConversionService(_mockConverter.Object, _mockValidator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ConvertAsync_ValidInput_ReturnsSuccessResult()
    {
        // Arrange
        var input = "123.45";
        var parsedValue = 123.45m;
        var convertedText = "ONE HUNDRED TWENTY THREE DOLLARS AND FORTY FIVE CENTS";

        _mockValidator.Setup(v => v.Validate(input))
            .Returns(ValidationResult.Success(parsedValue));
        
        _mockConverter.Setup(c => c.Convert(parsedValue, It.IsAny<ConversionOptions>()))
            .Returns(convertedText);

        // Act
        var result = await _service.ConvertAsync(input);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().Be(convertedText);
        result.OriginalValue.Should().Be(parsedValue);
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ConvertAsync_InvalidInput_ReturnsFailureResult()
    {
        // Arrange
        var input = "invalid";
        var errorMessage = "Invalid number format";

        _mockValidator.Setup(v => v.Validate(input))
            .Returns(ValidationResult.Failure(errorMessage));

        // Act
        var result = await _service.ConvertAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.Result.Should().BeNull();
        result.OriginalValue.Should().BeNull();
    }

    [Fact]
    public async Task ConvertAsync_WithOptions_PassesOptionsToConverter()
    {
        // Arrange
        var input = "123.45";
        var parsedValue = 123.45m;
        var convertedText = "One Hundred Twenty Three Dollars And Forty Five Cents";
        var options = new ConversionOptions { Style = CapitalizationStyle.TitleCase };

        _mockValidator.Setup(v => v.Validate(input))
            .Returns(ValidationResult.Success(parsedValue));
        
        _mockConverter.Setup(c => c.Convert(parsedValue, options))
            .Returns(convertedText);

        // Act
        var result = await _service.ConvertAsync(input, options);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().Be(convertedText);
        _mockConverter.Verify(c => c.Convert(parsedValue, options), Times.Once);
    }

    [Fact]
    public async Task ConvertAsync_ConverterThrowsNotImplementedException_ReturnsFailureResult()
    {
        // Arrange
        var input = "1000000";
        var parsedValue = 1000000m;
        var exceptionMessage = "Numbers greater than 999,999.99 not yet supported";

        _mockValidator.Setup(v => v.Validate(input))
            .Returns(ValidationResult.Success(parsedValue));
        
        _mockConverter.Setup(c => c.Convert(parsedValue, It.IsAny<ConversionOptions>()))
            .Throws(new NotImplementedException(exceptionMessage));

        // Act
        var result = await _service.ConvertAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(exceptionMessage);
        result.Result.Should().BeNull();
        result.OriginalValue.Should().BeNull();
    }

    [Fact]
    public async Task ConvertAsync_ConverterThrowsUnexpectedException_ReturnsGenericFailureResult()
    {
        // Arrange
        var input = "123.45";
        var parsedValue = 123.45m;

        _mockValidator.Setup(v => v.Validate(input))
            .Returns(ValidationResult.Success(parsedValue));
        
        _mockConverter.Setup(c => c.Convert(parsedValue, It.IsAny<ConversionOptions>()))
            .Throws(new InvalidOperationException("Unexpected error"));

        // Act
        var result = await _service.ConvertAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("An unexpected error occurred during conversion");
        result.Result.Should().BeNull();
        result.OriginalValue.Should().BeNull();
    }

    [Fact]
    public async Task ConvertAsync_WithNullInput_ReturnsFailureResult()
    {
        // Arrange
        string input = null!;
        var errorMessage = "Input cannot be empty";

        _mockValidator.Setup(v => v.Validate(input))
            .Returns(ValidationResult.Failure(errorMessage));

        // Act
        var result = await _service.ConvertAsync(input);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ConvertAsync_LogsInformationOnSuccess()
    {
        // Arrange
        var input = "123.45";
        var parsedValue = 123.45m;
        var convertedText = "ONE HUNDRED TWENTY THREE DOLLARS AND FORTY FIVE CENTS";

        _mockValidator.Setup(v => v.Validate(input))
            .Returns(ValidationResult.Success(parsedValue));
        
        _mockConverter.Setup(c => c.Convert(parsedValue, It.IsAny<ConversionOptions>()))
            .Returns(convertedText);

        // Act
        await _service.ConvertAsync(input);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting conversion")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully converted")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ConvertAsync_LogsWarningOnValidationFailure()
    {
        // Arrange
        var input = "invalid";
        var errorMessage = "Invalid number format";

        _mockValidator.Setup(v => v.Validate(input))
            .Returns(ValidationResult.Failure(errorMessage));

        // Act
        await _service.ConvertAsync(input);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Validation failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_NullConverter_ThrowsArgumentNullException()
    {
        // Act & Assert
        FluentActions.Invoking(() => new NumberConversionService(null!, _mockValidator.Object, _mockLogger.Object))
            .Should().Throw<ArgumentNullException>()
            .WithParameterName("converter");
    }

    [Fact]
    public void Constructor_NullValidator_ThrowsArgumentNullException()
    {
        // Act & Assert
        FluentActions.Invoking(() => new NumberConversionService(_mockConverter.Object, null!, _mockLogger.Object))
            .Should().Throw<ArgumentNullException>()
            .WithParameterName("validator");
    }

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        FluentActions.Invoking(() => new NumberConversionService(_mockConverter.Object, _mockValidator.Object, null!))
            .Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }
}