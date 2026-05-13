using BookStoreApi.Models.Database;
using BookStoreApi.Models.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreApi.Services;

/// <summary>
/// Service for managing books in the bookstore.
/// This service interacts with a MongoDB database to perform CRUD operations on the books collection.
/// </summary>
/// <param name="bookStoreDatabaseSettings">The database settings for the bookstore.</param>
public class BooksService(IOptions<BookStoreDatabaseOptions> bookStoreDatabaseSettings)
{
    private readonly IMongoCollection<Book> _booksCollection = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString)
        .GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName)
        .GetCollection<Book>(bookStoreDatabaseSettings.Value.BooksCollectionName);

    /// <summary>
    /// Retrieves all books from the database.
    /// </summary>
    /// <returns>A list of all books in the database.</returns>
    public async Task<List<Book>> GetAsync() =>
        await _booksCollection.Find(_ => true).ToListAsync();

    /// <summary>
    /// Retrieves a single book by its ID from the database.
    /// </summary>
    /// <param name="id">The ID of the book to retrieve.</param>
    /// <returns>The book with the specified ID, or null if not found.</returns>
    public async Task<Book?> GetAsync(string id) =>
        await _booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    /// <summary>
    /// Creates a new book in the database.
    /// </summary>
    /// <param name="newBook">The book to create.</param>
    public async Task CreateAsync(Book newBook) =>
        await _booksCollection.InsertOneAsync(newBook);

    /// <summary>
    /// Updates an existing book in the database.
    /// </summary>
    /// <param name="id">The ID of the book to update.</param>
    /// <param name="updatedBook">The updated book data.</param>
    public async Task UpdateAsync(string id, Book updatedBook) =>
        await _booksCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    /// <summary>
    /// Removes a book from the database.
    /// </summary>
    /// <param name="id">The ID of the book to remove.</param>
    public async Task RemoveAsync(string id) =>
        await _booksCollection.DeleteOneAsync(x => x.Id == id);
}