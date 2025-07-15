# Approach Documentation

## Overview

This document explains the algorithmic choices, implementation strategies, and decision-making process behind the Number to Words Converter. It covers the "why" behind the solution approach and technical implementation details.

## Core Algorithm: Recursive Number Breakdown

### Why Recursive Breakdown?

The core number-to-words conversion uses a recursive breakdown approach for several key reasons:

1. **Natural Language Structure**: English number words follow hierarchical patterns (thousands, hundreds, tens, ones)
2. **Maintainability**: Each level of magnitude is handled independently
3. **Extensibility**: Easy to add new ranges (millions, billions) by extending the pattern
4. **Clarity**: Algorithm mirrors how humans naturally think about numbers

### Algorithm Structure

```
ConvertWholeNumber(number)
├── Handle Thousands (1000-999999)
│   ├── Extract thousand part: number / 1000
│   ├── Recursively convert thousand part
│   ├── Add "THOUSAND"
│   └── Handle remainder: number % 1000
└── Handle Hundreds (0-999)
    ├── Extract hundred part: number / 100
    ├── Convert hundred part + "HUNDRED"
    └── Handle remainder (tens and ones)
        ├── Teens (10-19): Direct lookup
        ├── Tens (20-90): Lookup + ones
        └── Ones (1-9): Direct lookup
```

### Example Walkthrough: 123,456

```
ConvertWholeNumber(123456)
├── Thousands: 123456 / 1000 = 123
│   ├── ConvertHundreds(123)
│   │   ├── Hundreds: 123 / 100 = 1 → "ONE HUNDRED"
│   │   └── Remainder: 123 % 100 = 23 → "TWENTY THREE"
│   │   └── Result: "ONE HUNDRED TWENTY THREE"
│   ├── Add "THOUSAND"
│   └── Result: "ONE HUNDRED TWENTY THREE THOUSAND"
└── Remainder: 123456 % 1000 = 456
    ├── ConvertHundreds(456)
    │   ├── Hundreds: 456 / 100 = 4 → "FOUR HUNDRED"
    │   └── Remainder: 456 % 100 = 56 → "FIFTY SIX"
    │   └── Result: "FOUR HUNDRED FIFTY SIX"
    └── Final: "ONE HUNDRED TWENTY THREE THOUSAND FOUR HUNDRED FIFTY SIX"
```

## Input Validation Strategy

### Multi-Layer Validation Approach

We implement validation at multiple layers for defense in depth:

1. **Client-Side Validation**: Real-time feedback for user experience
2. **DTO Validation**: Data annotations on request models
3. **Business Logic Validation**: Domain-specific rules in validators
4. **Boundary Validation**: API-level input sanitization

### Validation Philosophy

**Fail Fast**: Invalid inputs are rejected as early as possible
- Prevents downstream errors
- Provides immediate feedback to users
- Reduces processing overhead

**Clear Error Messages**: Each validation provides specific guidance
- "Currency values cannot have more than 2 decimal places"
- "Value must be between 0 and 999,999.99"
- "Negative numbers are not currently supported"

### Specific Validation Rules

#### Decimal Places Validation
```csharp
// String-based validation for precision
if (input.Contains('.'))
{
    var decimalPart = input.Split('.')[1];
    if (decimalPart.Length > 2)
    {
        return ValidationResult.Failure("Currency values cannot have more than 2 decimal places");
    }
}
```

**Why String-Based**: 
- Decimal parsing can lose precision information
- User input should be validated before type conversion
- More accurate than binary decimal analysis

#### Range Validation
```csharp
private const decimal MaxValue = 999999.99m;
private const decimal MinValue = 0m;
```

**Why These Limits**:
- Maximum supports most real-world currency amounts
- Deliberately limited scope demonstrates incremental development
- Easy to extend when requirements expand

#### Number Format Validation
```csharp
NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign
```

**Why Restrictive**: 
- No thousands separators (commas) to avoid confusion
- No currency symbols to keep parsing simple
- Clear, unambiguous input format

## Currency Conversion Logic

### MoneyAmount Value Object

The `MoneyAmount` struct encapsulates money-specific logic:

```csharp
public readonly struct MoneyAmount
{
    public decimal Value { get; }
    public int DollarPart { get; }
    public int CentPart { get; }
}
```

### Rounding Strategy

**Banker's Rounding**: Uses `Math.Round()` default behavior
- Rounds 0.5 to nearest even number
- Reduces cumulative rounding bias
- Standard practice in financial applications

**Cent Carryover Logic**:
```csharp
if (cents >= 100)
{
    dollars += cents / 100;
    cents = cents % 100;
}
```

**Why Carryover**: Handles edge cases where rounding produces ≥100 cents
- Example: 123.995 rounds to 123 dollars, 100 cents → 124 dollars, 0 cents
- Maintains mathematical accuracy
- Prevents invalid money representations

### Singular/Plural Handling

```csharp
var centUnit = money.CentPart == 1 ? options.SubCurrencyName.TrimEnd('S') : options.SubCurrencyName;
```

**Approach**: Simple string manipulation for English pluralization
- "CENTS" → "CENT" for count of 1
- Works for standard English currency terms
- Extensible for other languages with more complex rules

## Error Handling Philosophy

### Exception Strategy

**Known Limitations**: Use `NotImplementedException` for planned features
```csharp
if (number < 0)
{
    throw new NotImplementedException("Negative numbers not yet supported");
}
```

**Benefits**:
- Clear communication of current limitations
- Easy to find and implement in future iterations
- Prevents silent failures or incorrect results

**Unexpected Errors**: Catch-all exception handling
```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error converting input: {Input}", input);
    return ConversionResult.Failure("An unexpected error occurred during conversion");
}
```

**Benefits**:
- Prevents application crashes
- Provides logging for debugging
- User-friendly error messages

### Logging Strategy

**Structured Logging**: Key events with context
- Input validation attempts
- Successful conversions with timing
- Error conditions with details

**Log Levels**:
- **Information**: Normal operation events
- **Warning**: Validation failures (user errors)
- **Error**: Unexpected exceptions (system errors)

## UI/UX Design Decisions

### Progressive Enhancement

**Base Functionality**: Works without JavaScript
- Form submission to API endpoint
- Server-side validation and response
- Graceful degradation for accessibility

**Enhanced Experience**: JavaScript for better UX
- Real-time validation feedback
- Loading states during processing
- Async API calls without page refresh

### Validation Feedback

**Client-Side Validation**: Immediate feedback as user types
```javascript
// Real-time validation
document.getElementById('numberInput').addEventListener('input', (e) => {
    const value = e.target.value;
    const regex = /^\d*\.?\d{0,2}$/;
    
    if (!regex.test(value)) {
        e.target.classList.add('is-invalid');
    } else {
        e.target.classList.remove('is-invalid');
    }
});
```

**Server-Side Validation**: Authoritative validation with detailed messages
- Client validation is convenience, not security
- Server validation is source of truth
- Detailed error messages help user correct input

### Response Design

**Performance Feedback**: Show processing time
- Demonstrates system responsiveness
- Useful for performance monitoring
- Builds user confidence in system speed

**Error Recovery**: Clear guidance for fixing issues
- Specific error messages with examples
- Positive reinforcement for correct inputs
- Progressive disclosure of advanced options

## Performance Considerations

### Algorithm Efficiency

**Time Complexity**: O(log n) where n is the input number
- Each recursive call handles one magnitude level
- Maximum 6 levels for supported range (ones, tens, hundreds, thousands, ten-thousands, hundred-thousands)
- Constant time for word lookup operations

**Space Complexity**: O(log n) for recursion stack
- Minimal memory usage for supported range
- String concatenation could be optimized for very high volume

### Optimization Opportunities (Deliberately Not Implemented)

**String Builder Optimization**: Could reduce memory allocations
```csharp
// Current approach: simple and readable
result.Append(ConvertWholeNumber(money.DollarPart));

// Optimized approach: pass StringBuilder through recursion
ConvertWholeNumber(money.DollarPart, result);
```

**Caching**: Could cache results for common values
- Most number conversions likely repeat frequently
- LRU cache could improve performance
- Balances memory usage vs. CPU time

**Why Not Implemented**: 
- Premature optimization avoided
- Code clarity prioritized
- Performance adequate for expected usage
- Easy to add later with profiling data

## Testing Strategy Rationale

### Test-First Mindset

**Comprehensive Coverage**: 111 tests across all components
- Domain logic thoroughly tested
- Edge cases explicitly covered
- Integration points validated

### Test Categories

**Unit Tests**: Isolated component testing
- Mock dependencies for pure unit testing
- Fast execution for rapid feedback
- Clear test names describing expected behavior

**Integration Tests**: Component interaction testing
- Real implementations working together
- API endpoint testing with actual HTTP calls
- Database integration (if added later)

**Property-Based Testing**: Implicit through comprehensive examples
- Boundary value testing (0, 999999.99)
- Equivalence class testing (valid/invalid inputs)
- Error condition testing (negative, too large, invalid format)

### Test Data Strategy

**Theory-Based Tests**: Multiple inputs per test method
```csharp
[Theory]
[InlineData(123.45, "ONE HUNDRED TWENTY THREE DOLLARS AND FORTY FIVE CENTS")]
[InlineData(1.01, "ONE DOLLAR AND ONE CENT")]
```

**Benefits**:
- Comprehensive coverage with minimal code
- Easy to add new test cases
- Clear documentation of expected behavior

## Future Enhancement Considerations

### Algorithmic Improvements

**Localization Support**: Different language implementations
- Strategy pattern already supports this
- Word arrays could be externalized
- Grammar rules could be pluggable

**Extended Range Support**: Millions, billions, etc.
- Current recursive structure extends naturally
- New word arrays needed
- Validation limits need updating

**Alternative Number Systems**: Roman numerals, spelled fractions
- Additional `INumberToWordsConverter` implementations
- Factory pattern for converter selection
- New `ConversionOptions` properties

### Performance Improvements

**Caching Layer**: For high-volume scenarios
- Redis cache for distributed scenarios
- In-memory cache for single instance
- Cache invalidation strategy needed

**Async Optimization**: For I/O bound operations
- Database lookups for word translations
- External service calls for validation
- Batch processing capabilities

### User Experience Enhancements

**Advanced UI Features**: History, favorites, bulk conversion
- Browser storage for user preferences
- Export functionality for results
- Batch file processing

**API Improvements**: Rate limiting, authentication, versioning
- OpenAPI specification improvements
- Webhook support for integrations
- GraphQL endpoint for flexible queries

This approach balances simplicity with extensibility, providing a solid foundation for future enhancements while maintaining clean, readable, and testable code.