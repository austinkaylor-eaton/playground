using System.Diagnostics.CodeAnalysis;

namespace Core.CQRS;

/// <summary>
/// Marker interface for queries that return a <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the query result.</typeparam>
[SuppressMessage("Design", "CA1040:Avoid empty interfaces",
    Justification = "Used as a generic type constraint in the mediator pipeline")]
[SuppressMessage("Design","S2326",
    Justification = "Used as a generic type constraint in the mediator pipeline")]
public interface IQuery<TResponse>;
