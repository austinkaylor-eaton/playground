namespace BookStoreApi.Models.Options;

/// <summary>
/// Represents the configuration options for the bookstore database, including connection string, database name, and collection name.
/// </summary>
/// <remarks>
/// <see href="https://learn.microsoft.com/en-us/dotnet/core/extensions/options">Options pattern in .NET</see> <br/>
/// </remarks>
public sealed class BookStoreDatabaseOptions
{
    /// <summary>
    /// The name of the root section in the configuration file (e.g., appsettings.json) that contains the bookstore database settings.
    /// </summary>
    public const string SectionName = "BookStoreDatabase";

    /// <summary>
    /// The connection string used to connect to the bookstore database.
    /// </summary>
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// The name of the bookstore database.
    /// </summary>
    public string DatabaseName { get; set; } = null!;

    /// <summary>
    /// The name of the collection that stores the books in the bookstore database.
    /// </summary>
    public string BooksCollectionName { get; set; } = null!;
}