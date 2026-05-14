using System.Diagnostics.CodeAnalysis;

namespace Core.CQRS;

/// <summary>
/// Marker interface for queries that return a <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the query result.</typeparam>
/// <seealso href="https://www.milanjovanovic.tech/blog/cqrs-pattern-the-way-it-should-have-been-from-the-start">CQRS Pattern - Milan Jovanović</seealso>
[SuppressMessage("Design", "CA1040:Avoid empty interfaces",
    Justification = "Used as a generic type constraint in the mediator pipeline")]
[SuppressMessage("Design","S2326",
    Justification = "Used as a generic type constraint in the mediator pipeline")]
public interface IQuery<TResponse>;
