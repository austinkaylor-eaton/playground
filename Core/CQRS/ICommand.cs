using System.Diagnostics.CodeAnalysis;

namespace Core.CQRS;

/// <summary>
/// Marker interface for commands that do not return a value.
/// </summary>
[SuppressMessage("Design", "CA1040:Avoid empty interfaces",
    Justification = "Used as a generic type constraint in the mediator pipeline")]
public interface ICommand;

/// <summary>
/// Marker interface for commands that return a <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the command result.</typeparam>
[SuppressMessage("Design", "CA1040:Avoid empty interfaces",
    Justification = "Used as a generic type constraint in the mediator pipeline")]
[SuppressMessage("Design","S2326",
    Justification = "Used as a generic type constraint in the mediator pipeline")]
public interface ICommand<TResponse>;