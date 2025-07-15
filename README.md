# Number to Words Converter

A professional C# web application that converts numerical input to words, demonstrating clean architecture and senior-level development practices.

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- Any modern web browser

### Build and Run
```bash
# Clone and navigate to project
cd NumberToWords

# Build the solution
dotnet build

# Run the web application
dotnet run --project src/NumberToWords.Web

# Run tests
dotnet test
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`.

## Features Implemented

- [x] **Core Number Conversion**: Supports numbers 0 to 999,999.99
- [x] **Currency Format**: Converts to "DOLLARS AND CENTS" format
- [x] **Input Validation**: Comprehensive validation with clear error messages
- [x] **Multiple Capitalization Styles**: ALL UPPER, Title Case, sentence case
- [x] **Clean Web Interface**: Professional Bootstrap-styled UI
- [x] **REST API**: `/api/convert` endpoint with OpenAPI documentation
- [x] **Real-time Validation**: Client-side input validation with immediate feedback
- [x] **Performance Monitoring**: Response timing included in API results
- [x] **Comprehensive Testing**: 111 unit tests with full coverage
- [x] **Structured Logging**: Serilog integration with file and console output
- [x] **Clean Architecture**: Proper separation of concerns and SOLID principles

## Usage Examples

### Web Interface
1. Navigate to the application URL
2. Enter a number (e.g., "123.45")
3. Click "Convert to Words"
4. View result: "ONE HUNDRED TWENTY THREE DOLLARS AND FORTY FIVE CENTS"

### API Usage
```bash
# Basic conversion
curl -X POST "http://localhost:5000/api/convert" \
  -H "Content-Type: application/json" \
  -d '{"input": "123.45"}'

# With custom options
curl -X POST "http://localhost:5000/api/convert" \
  -H "Content-Type: application/json" \
  -d '{
    "input": "123.45",
    "options": {
      "currencyName": "POUNDS",
      "subCurrencyName": "PENCE",
      "style": "TitleCase"
    }
  }'
```

### Example Conversions
| Input | Output |
|-------|--------|
| 0 | ZERO DOLLARS AND ZERO CENTS |
| 1 | ONE DOLLAR AND ZERO CENTS |
| 0.01 | ZERO DOLLARS AND ONE CENT |
| 123.45 | ONE HUNDRED TWENTY THREE DOLLARS AND FORTY FIVE CENTS |
| 1000.00 | ONE THOUSAND DOLLARS AND ZERO CENTS |
| 999999.99 | NINE HUNDRED NINETY NINE THOUSAND NINE HUNDRED NINETY NINE DOLLARS AND NINETY NINE CENTS |

## Project Structure

```
NumberToWords/
├── src/
│   ├── NumberToWords.Web/              # ASP.NET Core Web App & API
│   ├── NumberToWords.Application/      # Application Services & Orchestration
│   ├── NumberToWords.Domain/           # Core Business Logic & Models
│   └── NumberToWords.Infrastructure/   # External Dependencies & Implementations
├── tests/
│   └── NumberToWords.Tests.Unit/       # Comprehensive Unit Test Suite
├── docs/
│   ├── ARCHITECTURE.md                 # Architecture Decisions & Design
│   ├── APPROACH.md                     # Algorithmic Choices & Implementation
│   └── TEST-PLAN.md                    # Testing Strategy & Coverage
└── README.md                           # This file
```

## Technology Stack

- **Framework**: ASP.NET Core 8.0 (Minimal APIs + Razor Pages)
- **Frontend**: HTML5/CSS3/JavaScript with Bootstrap 5
- **Testing**: xUnit, FluentAssertions, Moq
- **Logging**: Serilog with structured logging
- **Documentation**: OpenAPI/Swagger
- **Architecture**: Clean Architecture with SOLID principles

## API Reference

### POST /api/convert

Converts a number to its word representation.

**Request Body:**
```json
{
  "input": "123.45",
  "options": {
    "currencyName": "DOLLARS",
    "subCurrencyName": "CENTS",
    "useAndConnector": true,
    "style": "AllUpper"
  }
}
```

**Response:**
```json
{
  "isSuccess": true,
  "result": "ONE HUNDRED TWENTY THREE DOLLARS AND FORTY FIVE CENTS",
  "originalValue": 123.45,
  "processingTimeMs": 15
}
```

**Validation Rules:**
- Input must be a valid decimal number
- Range: 0 to 999,999.99
- Maximum 2 decimal places
- No thousands separators or currency symbols

**Error Response:**
```json
{
  "isSuccess": false,
  "errorMessage": "Currency values cannot have more than 2 decimal places",
  "processingTimeMs": 5
}
```

## Development

### Architecture

This application follows Clean Architecture principles with clear separation of concerns:

- **Domain Layer**: Core business logic, value objects, and interfaces
- **Application Layer**: Use cases, orchestration, and application services  
- **Infrastructure Layer**: External dependencies and technical implementations
- **Presentation Layer**: Web UI and API endpoints

See [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for detailed design decisions.

### Testing

Comprehensive test suite with 111 tests covering:

- **Unit Tests**: All business logic components
- **Integration Tests**: API endpoints and service interactions
- **Edge Cases**: Boundary values, error conditions, and validation rules
- **Mocking**: Isolated testing with dependency injection

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test category
dotnet test --filter "FullyQualifiedName~CurrencyConverter"
```

### Configuration

Key configuration options in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "Serilog": {
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/app-.txt" } }
    ]
  }
}
```

### Adding New Features

The architecture supports easy extension:

1. **New Number Ranges**: Extend word arrays and validation limits
2. **Multiple Languages**: Implement new `INumberToWordsConverter`
3. **Additional Formats**: Add options to `ConversionOptions`
4. **New Currencies**: Extend configuration system

## Known Limitations & Future Enhancements

### Current Limitations
- [ ] Numbers > 999,999.99 not supported
- [ ] Negative number handling incomplete  
- [ ] Single currency type (USD-style)
- [ ] English language only

### Planned Enhancements
- [ ] Support for millions, billions, trillions
- [ ] Multiple currency formats (EUR, GBP, etc.)
- [ ] Localization for different languages
- [ ] Performance optimization for high-volume usage
- [ ] Advanced UI features (history, bulk conversion)
- [ ] Integration with external currency services
- [ ] Containerization with Docker
- [ ] CI/CD pipeline configuration

### Performance Considerations
- Current implementation optimized for readability over performance
- Algorithm complexity: O(log n) for conversion
- Memory usage minimal for supported range
- Can handle typical web application load

## Contributing

### Code Standards
- Follow Clean Architecture principles
- Maintain comprehensive test coverage
- Use structured logging for observability
- Document architectural decisions
- Keep SOLID principles in mind

### Pull Request Process
1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request with description

### Development Setup
```bash
# Install .NET 8.0 SDK
# Clone repository
git clone <repository-url>
cd NumberToWords

# Restore dependencies
dotnet restore

# Run in development mode
dotnet run --project src/NumberToWords.Web
```

## Troubleshooting

### Common Issues

**Build Errors:**
- Ensure .NET 8.0 SDK is installed
- Run `dotnet restore` to restore packages
- Check for missing project references

**Test Failures:**
- Verify all packages are restored
- Check for environment-specific issues
- Run tests in isolation to identify conflicts

**Runtime Issues:**
- Check application logs in `logs/` directory
- Verify port availability (5000/5001)
- Ensure proper configuration in `appsettings.json`

### Debug Mode
```bash
# Run with detailed logging
dotnet run --project src/NumberToWords.Web --verbosity detailed

# Run tests with diagnostic output
dotnet test --logger "console;verbosity=detailed"
```

## License

This project is developed as a technical demonstration for TechnologyOne assessment purposes.

## Contact

For questions about this implementation approach or architectural decisions, please refer to the documentation in the `docs/` directory or create an issue in the repository.

---

**Built with ❤️ using Clean Architecture principles and senior-level development practices.**