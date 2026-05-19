# Core Library

⬅️ [Back to Solution](../README.md)

A .NET 10.0 shared library providing Domain-Driven Design (DDD) building blocks and a
CQRS (Command Query Responsibility Segregation) framework with cross-cutting concerns
handled via the decorator pattern.

## Overview

The Core library serves as a foundational framework for building enterprise applications
with clean architecture patterns. It provides:

- Rich domain entity base classes with composable behavioral contracts
- CQRS pipeline with strongly-typed command and query handlers
- Decorator-based middleware for logging, tracing, metrics, timeout, and validation
- Railway-oriented Result pattern for explicit error handling
- OpenTelemetry-ready observability baked into the pipeline

## Project Structure

```
Core/
├── Entity.cs                # Base entity class with domain event support
├── DomainEvent.cs           # Abstract domain event record
├── Result.cs                # Result pattern (success/failure)
├── ResultError.cs           # Error type for Result
├── IRepository.cs           # Generic repository interface
├── IActivateEntity.cs       # Active/inactive toggle contract
├── IAuditEntity.cs          # Audit trail contract
├── ISoftDeleteEntity.cs     # Soft delete contract
├── ITenantEntity.cs         # Multi-tenancy contract
├── IVersionedEntity.cs      # Optimistic concurrency contract
├── IEntityId.cs             # Strongly-typed entity identifier
├── IDomainEvent.cs          # Domain event marker interface
├── IHaveDomainEvents.cs     # Domain event collection contract
├── CQRS/
│   ├── ICommand.cs          # Command marker interfaces
│   ├── ICommandHandler.cs   # Command handler interfaces
│   ├── IQuery.cs            # Query marker interface
│   ├── IQueryHandler.cs     # Query handler interface
│   ├── Unit.cs              # Void-equivalent return type
│   ├── CqrsActivitySource.cs   # OpenTelemetry tracing
│   ├── CqrsMetrics.cs          # Metrics (counters/histograms)
│   ├── Log.cs                   # Source-generated structured logging
│   ├── TimeoutAttribute.cs      # Timeout configuration attribute
│   ├── Decorators/              # Cross-cutting concern decorators
│   │   ├── CommandHandlerLoggingDecorator.cs
│   │   ├── CommandHandlerMetricsDecorator.cs
│   │   ├── CommandHandlerTimeoutDecorator.cs
│   │   ├── CommandHandlerTracingDecorator.cs
│   │   ├── CommandHandlerValidationDecorator.cs
│   │   ├── QueryHandlerLoggingDecorator.cs
│   │   ├── QueryHandlerMetricsDecorator.cs
│   │   ├── QueryHandlerTimeoutDecorator.cs
│   │   ├── QueryHandlerTracingDecorator.cs
│   │   └── QueryHandlerValidationDecorator.cs
│   └── Validation/              # Validation pipeline
│       ├── ICommandValidator.cs
│       ├── IQueryValidator.cs
│       ├── ValidationError.cs
│       └── ValidationResultError.cs
└── V1/                          # Legacy CQRS interfaces (simplified)
    ├── ICommand.cs
    ├── ICommandHandler.cs
    ├── IQuery.cs
    └── IQueryHandler.cs
```

## Entity Patterns

The library provides a composable set of interfaces that can be mixed into domain
entities to add standard behaviors:

| Interface | Purpose |
| --------- | ------- |
| `IEntityId` | Strongly-typed entity identifier (`Guid Value`) |
| `IActivateEntity` | Active/inactive toggle |
| `IAuditEntity` | Audit trail (`CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`) |
| `ISoftDeleteEntity` | Soft delete (`IsDeleted`, `DeletedAt`, `DeletedBy`) |
| `ITenantEntity` | Multi-tenancy (`TenantId`) |
| `IVersionedEntity` | Optimistic concurrency (`Version`) |
| `IHaveDomainEvents` | Domain event collection and dispatch |

### Usage Example

```csharp
public record OrderId(Guid Value) : IEntityId;

public class Order : Entity<OrderId>, IAuditEntity, ISoftDeleteEntity
{
    public string CustomerName { get; set; } = string.Empty;

    // IAuditEntity
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // ISoftDeleteEntity
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
```

## CQRS Pipeline

### Commands

Commands represent intent to change state. They return a `Result` or `Result<T>`:

```csharp
public record CreateOrderCommand(string CustomerName) : ICommand<OrderId>;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderId>
{
    public async Task<Result<OrderId>> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        // Implementation
        var orderId = new OrderId(Guid.NewGuid());
        return Result<OrderId>.Success(orderId);
    }
}
```

### Queries

Queries represent requests for data without side effects:

```csharp
public record GetOrderQuery(OrderId Id) : IQuery<OrderDto>;

public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderDto>
{
    public async Task<Result<OrderDto>> HandleAsync(
        GetOrderQuery query,
        CancellationToken cancellationToken = default)
    {
        // Implementation
    }
}
```

### Decorators

Handlers are wrapped in decorators that provide cross-cutting behavior in a composable
pipeline:

| Decorator | Behavior |
| --------- | -------- |
| `LoggingDecorator` | Structured logging of handler start, success, and failure |
| `MetricsDecorator` | Execution count and duration via `System.Diagnostics.Metrics` |
| `TracingDecorator` | OpenTelemetry `Activity` spans with type metadata |
| `TimeoutDecorator` | Configurable timeout via `[Timeout]` attribute (default 30s) |
| `ValidationDecorator` | Runs validators before execution; short-circuits on failure |

### Validation

Implement `ICommandValidator<T>` or `IQueryValidator<T>` to add input validation:

```csharp
public class CreateOrderCommandValidator : ICommandValidator<CreateOrderCommand>
{
    public Task<IEnumerable<ValidationError>> ValidateAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(command.CustomerName))
            errors.Add(new ValidationError("CustomerName", "Customer name is required."));

        return Task.FromResult<IEnumerable<ValidationError>>(errors);
    }
}
```

## Result Pattern

The library uses a Result pattern for explicit error handling without exceptions:

```csharp
// Non-generic Result (success or failure)
Result success = Result.Success();
Result failure = Result.Failure(new ResultError("NOT_FOUND", "Order not found."));

// Generic Result<T> (success with value or failure)
Result<Order> found = Result<Order>.Success(order);
Result<Order> notFound = Result<Order>.Failure(new ResultError("NOT_FOUND", "Order not found."));

// Checking results
if (result.IsSuccess)
{
    var value = result.Value;
}

if (result.IsFailure)
{
    var error = result.Error; // ResultError with Code and Message
}
```

## Repository Pattern

A generic repository interface provides standard persistence operations:

```csharp
public interface IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : IEntityId
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    IQueryable<TEntity> Query { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

## Dependencies

- **`System.Diagnostics.DiagnosticSource`** — Provides `ActivitySource` and `Meter`
  for OpenTelemetry-compatible distributed tracing and metrics collection.

## Getting Started

Reference the Core project from your application:

```xml
<ProjectReference Include="..\Core\Core.csproj" />
```

Register your handlers and decorators in the DI container. The decorator order
determines the pipeline execution sequence (outermost runs first).

