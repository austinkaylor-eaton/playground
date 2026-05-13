namespace BookStoreApi.Models;

/// <summary>
/// Represents the currency used for pricing books in the bookstore.
/// </summary>
/// <remarks>
/// Directly used by the <see cref="Money"/> struct
/// </remarks>
public enum Currency
{
    USD, // US Dollar
    EUR, // Euro
    GBP, // British Pound
    JPY, // Japanese Yen
    AUD, // Australian Dollar
    CAD, // Canadian Dollar
    CHF, // Swiss Franc
    CNY, // Chinese Yuan
    SEK, // Swedish Krona
    NZD  // New Zealand Dollar
}