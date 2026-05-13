using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStoreApi.Models.Database;

/// <summary>
/// Represents a book in the bookstore.
/// </summary>
/// <remarks>
/// This class is used to map the book data stored in the MongoDB database.
/// </remarks>
public class Book
{
    /// <summary>
    /// A unique identifier for the book.
    /// </summary>
    /// <remarks>
    /// This is the primary key in the MongoDB collection.
    /// </remarks>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// The name of the book.
    /// </summary>
    [BsonElement("Name")]
    [JsonPropertyName("Name")]
    public string BookName { get; set; } = null!;

    /// <summary>
    /// The price of the book.
    /// </summary>
    [BsonElement("Price")]
    [JsonPropertyName("Price")]
    public decimal Price { get; set; }

    /// <summary>
    /// The category of the book.
    /// </summary>
    [BsonElement("Category")]
    [JsonPropertyName("Category")]
    public string Category { get; set; } = null!;

    /// <summary>
    /// The author of the book.
    /// </summary>
    [BsonElement("Author")]
    [JsonPropertyName("Author")]
    public string Author { get; set; } = null!;
}