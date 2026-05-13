using System.ComponentModel.DataAnnotations;
using BookStoreApi.Models.Database;

namespace BookStoreApi.Models.DataTransferObjects;

public class BookDTO
{
    /// <inheritdoc cref="Book.BookName"/>
    [StringLength(250, ErrorMessage = "Book name cannot exceed 250 characters.")]
    public required string BookName { get; set; }

    /// <inheritdoc cref="Book.Price"/>
    public Money Price { get; set; }

    /// <inheritdoc cref="Book.Category"/>
    public BookCategory Category { get; set; } = BookCategory.Uncategorized;

    /// <inheritdoc cref="Book.Author"/>
    public required string Author { get; set; }

    /// <inheritdoc cref="CreateBookFromBookDTO"/>
    public static implicit operator Book(BookDTO dto) => CreateBookFromBookDTO(dto);

    /// <inheritdoc cref="CreateBookDTOFromBook"/>
    public static implicit operator BookDTO(Book book) => CreateBookDTOFromBook(book);

    /// <inheritdoc cref="CreateBookFromBookDTO"/>
    public Book ToBook() { return CreateBookFromBookDTO(this); }

    /// <inheritdoc cref="CreateBookDTOFromBook"/>
    public BookDTO ToBookDTO() { return CreateBookDTOFromBook(this); }

    /// <summary>
    /// Creates a new instance of the <see cref="Book"/> class based on the provided <see cref="BookDTO"/>.
    /// </summary>
    /// <param name="dto">The <see cref="BookDTO"/> to convert.</param>
    /// <returns>A new instance of the <see cref="Book"/> class.</returns>
    private static Book CreateBookFromBookDTO(BookDTO dto) =>
        new ()
        {
            BookName = dto.BookName,
            Price = dto.Price.Amount,
            Category = dto.Category.ToString(),
            Author = dto.Author
        };

    /// <summary>
    /// Creates a new instance of the <see cref="BookDTO"/> class based on the provided <see cref="Book"/>.
    /// </summary>
    /// <param name="book">The <see cref="Book"/> to convert.</param>
    /// <returns>A new instance of the <see cref="BookDTO"/> class.</returns>
    private static BookDTO CreateBookDTOFromBook(Book book) =>
        new()
        {
            BookName = book.BookName,
            Price = new Money(book.Price, CurrencyCode.USD), // Assuming USD for simplicity
            Category = Enum.TryParse<BookCategory>(book.Category, out var category) ? category : BookCategory.Uncategorized,
            Author = book.Author
        };
}