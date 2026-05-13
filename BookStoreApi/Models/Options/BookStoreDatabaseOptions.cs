namespace BookStoreApi.Models.Options;

/// <summary>
/// Represents the configuration options for the bookstore database, including connection string, database name, and collection name.
/// </summary>
public class BookStoreDatabaseOptions
{
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