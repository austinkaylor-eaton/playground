using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models;

/// <summary>
/// Initializes a new instance of the <see cref="Money"/> struct.
/// </summary>
/// <param name="amount">The amount of money.</param>
/// <param name="currency">The currency code.</param>
public struct Money(decimal amount,  Currency currency) : IEquatable<Money>
{
    /// <summary>
    /// Gets the amount of money.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be a non-negative value.")]
    public decimal Amount { get; } = amount;

    /// <summary>
    /// Gets the currency code (e.g., "USD", "EUR").
    /// </summary>
    public Currency Currency { get; } = currency;

    /// <inheritdoc/>
    public override readonly int GetHashCode() => HashCode.Combine(Amount, Currency);

    /// <summary>
    /// Determines whether two <see cref="Money"/> instances are equal by comparing their amount and currency.
    /// </summary>
    /// <param name="left">The first <see cref="Money"/> instance to compare.</param>
    /// <param name="right">The second <see cref="Money"/> instance to compare.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Money left, Money right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Money"/> instances are not equal by comparing their amount and currency.
    /// </summary>
    /// <param name="left">The first <see cref="Money"/> instance to compare.</param>
    /// <param name="right">The second <see cref="Money"/> instance to compare.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Money left, Money right) => !(left == right);

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="Money"/> instance by comparing their amount and currency.
    /// </summary>
    /// <param name="obj">The object to compare with the current <see cref="Money"/> instance.</param>
    /// <returns><c>true</c> if the specified object is equal to the current <see cref="Money"/> instance; otherwise, <c>false</c>.</returns>
    public override readonly bool Equals(object? obj)
    {
        if (obj is Money money)
        {
            return Equals(money);
        }
        return false;
    }

    /// <summary>
    /// Determines whether the specified <see cref="Money"/> instance is equal to the current instance by comparing their amount and currency.
    /// </summary>
    /// <param name="other">The <see cref="Money"/> instance to compare with the current instance.</param>
    /// <returns><c>true</c> if the specified <see cref="Money"/> instance is equal to the current instance; otherwise, <c>false</c>.</returns>
    public readonly bool Equals(Money other) => Amount == other.Amount && Currency == other.Currency;
}