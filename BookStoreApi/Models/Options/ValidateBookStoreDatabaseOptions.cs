using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreApi.Models.Options;

/// <summary>
/// Validates the <see cref="BookStoreDatabaseOptions"/> to ensure that the configuration values are correct and meet the required criteria.
/// </summary>
/// <param name="config">The configuration instance used to validate the options.</param>
[SuppressMessage("Design","CA1062:Validate arguments of public methods", Justification = "The configuration is provided by the framework and is expected to be non-null.")]
public sealed class ValidateBookStoreDatabaseOptions(IConfiguration config)
    : IValidateOptions<BookStoreDatabaseOptions>
{
    /// <summary>
    /// Gets the <see cref="BookStoreDatabaseOptions"/> instance that contains the configuration values for the bookstore database, which will be validated by this class.
    /// </summary>
    public BookStoreDatabaseOptions? Options { get; private set; } =
        config.GetSection(BookStoreDatabaseOptions.SectionName)
            .Get<BookStoreDatabaseOptions>();

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, BookStoreDatabaseOptions options)
    {
        StringBuilder? failure = new();

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            failure.AppendLine(
                $"{nameof(options.ConnectionString)} is required.");
        }
        else
        {
            try
            {
                // MongoUrl parses and validates the full MongoDB connection string format
                // (mongodb:// and mongodb+srv:// schemes, credentials, hosts, and options).
                _ = new MongoUrl(options.ConnectionString);
            }
            catch (MongoConfigurationException ex)
            {
                failure.AppendLine(
                    string.Create(CultureInfo.InvariantCulture,
                        $"{nameof(options.ConnectionString)} is not a valid MongoDB connection string: {ex.Message}"));
            }
        }

        if (string.IsNullOrWhiteSpace(options.DatabaseName))
        {
            failure.AppendLine(
                $"{nameof(options.DatabaseName)} is required.");
        }

        if (string.IsNullOrWhiteSpace(options.BooksCollectionName))
        {
            failure.AppendLine(
                $"{nameof(options.BooksCollectionName)} is required.");
        }

        return failure is { Length: 0 }
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failure.ToString());
    }
}