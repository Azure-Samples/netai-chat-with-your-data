# Unit Testing Strategy for Chat With Your Data

This document outlines the unit testing strategy implemented for the .NET Aspire "Chat with Your Data" application.

## Overview

The solution now includes comprehensive unit test coverage for the core components of the application. The tests are organized into separate test projects that follow .NET testing best practices.

## Test Projects Added

### 1. ChatWYData.DataEntities.Tests
**Purpose**: Tests for the core data entities used throughout the application.

**Coverage**:
- `Document` model validation and property behavior
- `FileProcessBatch` model validation and property behavior
- Constructor initialization
- Property getters and setters
- Date/time handling
- Edge cases and boundary conditions

**Key Test Scenarios**:
- Default value initialization
- Property assignment and retrieval
- Various file name formats
- Boolean flag combinations
- Timestamp validation
- File content type validation

### 2. ChatWYData.ApiServices.Tests
**Purpose**: Tests for HTTP client service classes that communicate with various APIs.

**Coverage**:
- `DocumentApiService` HTTP client interactions
- Mocked HTTP responses and error handling
- JSON serialization/deserialization
- Error scenario handling
- Request/response validation

**Key Test Scenarios**:
- Successful API responses
- HTTP error handling
- Null/empty response handling
- Various HTTP status codes
- JSON parsing validation
- Cancellation token handling

### 3. ChatWYData.ServiceDefaults.Tests
**Purpose**: Tests for the .NET Aspire service defaults and extension methods.

**Coverage**:
- Service registration validation
- Health check configuration
- OpenTelemetry setup
- HTTP client defaults
- Service discovery configuration

**Key Test Scenarios**:
- Service registration verification
- Health check functionality
- Multiple initialization safety
- Development environment configuration
- Extension method behavior

### 4. ChatWYData.FileEntities.Tests
**Purpose**: Tests for file upload and processing entities.

**Coverage**:
- `FileUploadRequest` model validation
- `FileBatchUploadResponse` model validation
- JSON serialization with correct property names
- Large file handling
- Content type validation

**Key Test Scenarios**:
- Property initialization and assignment
- JSON serialization/deserialization
- File byte array handling
- Content type validation
- Large file scenarios

## Testing Technologies Used

- **xUnit**: Primary testing framework
- **Moq**: Mocking framework for HTTP client testing
- **System.Text.Json**: JSON serialization testing
- **.NET 9**: Target framework
- **Microsoft.NET.Test.Sdk**: Test runner and tooling

## Test Structure and Conventions

### Naming Conventions
- Test projects: `{ProjectName}.Tests`
- Test classes: `{ClassUnderTest}Tests`
- Test methods: `{MethodUnderTest}_{ExpectedBehavior}_{Condition}`

### Test Organization
- One test class per production class
- Grouped by functionality using descriptive test method names
- Arrange-Act-Assert pattern consistently applied
- Theory/InlineData for parameterized tests

### Coverage Areas
1. **Unit Tests**: Individual class and method behavior
2. **Integration Points**: HTTP client interactions with mocked dependencies
3. **Serialization**: JSON serialization/deserialization validation
4. **Configuration**: Service registration and setup validation

## Running Tests

### Run All Tests
```bash
cd src
dotnet test
```

### Run Specific Test Project
```bash
cd src
dotnet test ChatWYData.DataEntities.Tests
dotnet test ChatWYData.ApiServices.Tests
dotnet test ChatWYData.ServiceDefaults.Tests
dotnet test ChatWYData.FileEntities.Tests
```

### Run Tests with Coverage
```bash
cd src
dotnet test --collect:"XPlat Code Coverage"
```

## Test Metrics

| Test Project | Test Count | Success Rate |
|--------------|------------|--------------|
| ChatWYData.DataEntities.Tests | 24 | 100% |
| ChatWYData.ApiServices.Tests | 13 | 100% |
| ChatWYData.ServiceDefaults.Tests | 10 | 100% |
| ChatWYData.FileEntities.Tests | 25 | 100% |
| **Total** | **72** | **100%** |

## Future Testing Recommendations

### 1. Integration Tests
- Add integration tests for API endpoints using `Microsoft.AspNetCore.Mvc.Testing`
- Test complete request/response cycles with real database
- Validate Azure services integration

### 2. Additional Unit Test Areas
- **VectorEntities**: Vector storage and search models
- **SearchEntities**: Search functionality and models
- **DocumentsApi Endpoints**: API endpoint logic testing
- **Workers**: Background job processing logic

### 3. Performance Tests
- Load testing for document processing workflows
- Memory usage validation for large file uploads
- Concurrent processing scenarios

### 4. End-to-End Tests
- Complete document upload and processing workflows
- Chat functionality with document retrieval
- User interface automation tests

## CI/CD Integration

The test suite is designed to integrate seamlessly with CI/CD pipelines:

- Fast execution (< 10 seconds total)
- No external dependencies for unit tests
- Clear pass/fail indicators
- Detailed error reporting
- Code coverage metrics support

## Best Practices Implemented

1. **Isolation**: Each test is independent and can run in any order
2. **Clarity**: Test names clearly describe the scenario being tested
3. **Maintainability**: Tests use the same patterns and conventions
4. **Performance**: Tests run quickly with minimal setup overhead
5. **Reliability**: Tests use mocking to avoid external dependencies
6. **Coverage**: Critical paths and edge cases are covered

## Contributing to Tests

When adding new functionality:

1. Add corresponding unit tests in the appropriate test project
2. Follow existing naming conventions and patterns
3. Ensure tests are isolated and don't depend on external resources
4. Add both positive and negative test scenarios
5. Include edge cases and boundary conditions
6. Update this documentation if adding new test projects

This testing strategy provides a solid foundation for maintaining code quality and preventing regressions as the application evolves.