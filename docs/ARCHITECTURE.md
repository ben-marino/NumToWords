# Architecture Documentation

## Overview

This document outlines the architectural decisions made for the Number to Words Converter application, designed as a demonstration of clean architecture principles and senior-level development practices.

## Architectural Pattern: Clean Architecture

### Why Clean Architecture?

We chose Clean Architecture (also known as Onion Architecture) for several key reasons:

1. **Separation of Concerns**: Clear boundaries between business logic, application services, and infrastructure
2. **Testability**: Easy to unit test business logic in isolation
3. **Maintainability**: Changes in external dependencies don't affect core business logic
4. **Scalability**: Architecture supports future expansion and requirements changes
5. **Technology Independence**: Core domain logic is not coupled to specific frameworks or databases

### Layer Structure

```
┌─────────────────────────────────────────┐
│              Presentation               │
│          (NumberToWords.Web)            │
│    ┌─────────────────────────────────┐  │
│    │         Application             │  │
│    │   (NumberToWords.Application)   │  │
│    │  ┌───────────────────────────┐  │  │
│    │  │        Domain             │  │  │
│    │  │ (NumberToWords.Domain)    │  │  │
│    │  └───────────────────────────┘  │  │
│    └─────────────────────────────────┘  │
│    ┌─────────────────────────────────┐  │
│    │      Infrastructure             │  │
│    │ (NumberToWords.Infrastructure)  │  │
│    └─────────────────────────────────┘  │
└─────────────────────────────────────────┘
```

## Layer Responsibilities

### Domain Layer (NumberToWords.Domain)
**Purpose**: Contains core business logic and rules

**Components**:
- **Interfaces**: `INumberToWordsConverter`, `IInputValidator`
- **Value Objects**: `MoneyAmount` with proper rounding logic
- **Models**: `ConversionOptions`, `ValidationResult`, `ConversionResult`
- **Enums**: `CapitalizationStyle` for output formatting

**Key Decisions**:
- Value objects are immutable (`readonly struct`)
- No external dependencies - pure business logic
- Domain models contain validation logic (e.g., MoneyAmount handles cent carryover)

### Application Layer (NumberToWords.Application)
**Purpose**: Orchestrates business operations and enforces application rules

**Components**:
- **Services**: `NumberConversionService` - main application workflow
- **Interfaces**: `INumberConversionService` for abstraction
- **Dependency Injection**: Service registration extensions

**Key Decisions**:
- Async operations for future scalability
- Comprehensive logging for observability
- Exception handling with proper error messages
- Dependency injection for loose coupling

### Infrastructure Layer (NumberToWords.Infrastructure)
**Purpose**: Implements technical concerns and external dependencies

**Components**:
- **Converters**: `CurrencyConverter` - core algorithm implementation
- **Validators**: `CurrencyInputValidator` - input validation logic
- **Extensions**: Service registration for DI container

**Key Decisions**:
- Stateless services for thread safety
- Comprehensive validation with detailed error messages
- Deliberate limitations documented in code (TODOs for future work)

### Presentation Layer (NumberToWords.Web)
**Purpose**: Handles user interface and API endpoints

**Components**:
- **API**: Minimal API endpoints with proper HTTP status codes
- **Web Interface**: Razor Pages with clean HTML/CSS/JavaScript
- **Models**: DTOs for request/response mapping
- **Configuration**: Dependency injection setup and middleware

**Key Decisions**:
- Minimal APIs for simplicity and performance
- CORS enabled for potential SPA integration
- Structured logging with Serilog
- Performance timing in API responses

## SOLID Principles Applied

### Single Responsibility Principle (SRP)
- Each class has one clear responsibility
- `CurrencyConverter`: only converts numbers to words
- `CurrencyInputValidator`: only validates input
- `NumberConversionService`: only orchestrates conversion workflow

### Open/Closed Principle (OCP)
- Interfaces allow extension without modification
- New currency types can be added by implementing `INumberToWordsConverter`
- Additional validation rules via new `IInputValidator` implementations

### Liskov Substitution Principle (LSP)
- All implementations properly fulfill their interface contracts
- Mock objects in tests substitute real implementations seamlessly

### Interface Segregation Principle (ISP)
- Small, focused interfaces
- Clients only depend on methods they actually use
- No "fat" interfaces with unused methods

### Dependency Inversion Principle (DIP)
- High-level modules don't depend on low-level modules
- Application layer depends on domain interfaces, not concrete implementations
- Dependency injection enforces this at runtime

## Design Patterns Used

### Strategy Pattern
- `INumberToWordsConverter` allows different conversion strategies
- `ConversionOptions` provides configuration without changing core logic

### Dependency Injection
- Constructor injection for all dependencies
- Service locator pattern avoided
- Explicit dependency graph in startup configuration

### Value Object Pattern
- `MoneyAmount` encapsulates money-specific logic
- Immutable structures prevent invalid state
- Built-in validation and formatting

### DTO Pattern
- Clear separation between domain models and API contracts
- `ConvertRequest`/`ConvertResponse` for API boundary
- Prevents over-posting and under-posting issues

## Technology Choices

### ASP.NET Core 8.0
**Reasoning**: 
- Latest LTS version with improved performance
- Minimal APIs for simple, fast endpoints
- Built-in dependency injection and logging
- Excellent tooling and community support

### Serilog
**Reasoning**:
- Structured logging superior to built-in logging
- Easy configuration and multiple sinks
- Better observability for production environments

### xUnit + FluentAssertions + Moq
**Reasoning**:
- xUnit: Modern, extensible testing framework
- FluentAssertions: Readable, expressive test assertions
- Moq: Powerful mocking for isolated unit tests

## Trade-offs and Constraints

### Time Constraints
**Decisions Made**:
- Focused on core functionality over advanced features
- Minimal UI instead of full SPA framework
- Basic error handling instead of comprehensive fault tolerance

**Future Improvements**:
- More sophisticated error handling
- Performance optimizations for high volume
- Additional number ranges and currencies
- Advanced UI features

### Complexity vs. Simplicity
**Decisions Made**:
- Clean Architecture for demonstration purposes
- Some over-engineering for a simple converter
- Clear separation of concerns despite small codebase

**Justification**:
- Demonstrates senior-level architectural thinking
- Shows how to structure larger applications
- Provides foundation for future expansion

### Performance vs. Maintainability
**Decisions Made**:
- Prioritized code clarity over micro-optimizations
- Used async/await even where not strictly necessary
- Comprehensive logging might impact performance

**Justification**:
- Premature optimization avoided
- Code readability and maintainability prioritized
- Performance can be optimized later with profiling

## Extension Points

### Adding New Number Ranges
1. Extend `CurrencyConverter` with new word arrays
2. Update validation limits in `CurrencyInputValidator`
3. Add corresponding unit tests

### Supporting Multiple Languages
1. Create language-specific implementations of `INumberToWordsConverter`
2. Add language parameter to `ConversionOptions`
3. Register multiple converters in DI container

### Adding New Output Formats
1. Extend `CapitalizationStyle` enum
2. Update formatting logic in `CurrencyConverter`
3. Add new options to `ConversionOptions`

### Integration with External Systems
1. Infrastructure layer provides natural boundary
2. Add new implementations without changing core logic
3. Configuration-driven selection of implementations

## Security Considerations

### Input Validation
- Strict input validation prevents injection attacks
- No dynamic code execution or eval usage
- Decimal parsing with controlled format restrictions

### Dependency Management
- All packages from trusted sources (NuGet official feed)
- Regular security updates should be applied
- No packages with known vulnerabilities

### Logging Security
- No sensitive data logged
- Input values logged for debugging (safe for this application)
- Structured logging prevents log injection

## Monitoring and Observability

### Logging Strategy
- Structured logging with Serilog
- Multiple log levels (Information, Warning, Error)
- Correlation IDs for request tracking
- Performance timing included in responses

### Health Checks
- Basic application health via endpoints
- Database health checks could be added
- Custom health checks for external dependencies

### Metrics
- Built-in ASP.NET Core metrics
- Custom performance counters could be added
- Application-specific business metrics available

This architecture provides a solid foundation for a production-ready application while demonstrating best practices for maintainable, testable, and scalable code.