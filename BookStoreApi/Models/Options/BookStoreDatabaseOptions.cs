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
    /// <remarks>
    /// Using the ConfigurationKeyName attribute allows overriding the default behavior of binding configuration properties, which typically match the property name. <br/>
    /// In this case, it explicitly specifies that the configuration key for this property is "ConnectionString",
    /// which can be useful if the property name in the class differs from the key in the configuration file,
    /// or to ensure consistency in naming conventions across different parts of the application. <br/>
    /// See <see href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-10.0#specify-a-custom-key-name-for-a-configuration-property-using-configurationkeyname">Specify a custom key name for a configuration property using ConfigurationKeyName</see> for more details.
    /// </remarks>
    [ConfigurationKeyName("ConnectionString")]
    public required string ConnectionString { get; set; }

    /// <summary>
    /// The name of the bookstore database.
    /// </summary>
     public required string DatabaseName { get; set; }

    /// <summary>
    /// The name of the collection that stores the books in the bookstore database.
    /// </summary>
    public required string BooksCollectionName { get; set; }
}