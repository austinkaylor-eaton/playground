using BookStoreApi.Models.Database;
using BookStoreApi.Models.Options;
using BookStoreApi.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<BookStoreDatabaseOptions>(
    builder.Configuration.GetSection(BookStoreDatabaseOptions.SectionName));

builder.Services.AddSingleton<IValidateOptions<BookStoreDatabaseOptions>,
    ValidateBookStoreDatabaseOptions>();

builder.Services.AddSingleton<BooksService>();

// property names in the web API's serialized JSON response match their corresponding property names in the CLR object type.
// For example, the Book class's Author property serializes as Author instead of author
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.PropertyNamingPolicy = null);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

var booksGroup = app.MapGroup("/books");

booksGroup.MapGet("/", async (BooksService booksService) =>
{
    var books = await booksService.GetAsync();
    return Results.Ok(books);
});

booksGroup.MapGet("/{id:length(24)}", async (string id, BooksService booksService) =>
{
    var book = await booksService.GetAsync(id);

    return book is null ? Results.NotFound() : Results.Ok(book);
});

booksGroup.MapPost("/", async (Book newBook, BooksService booksService) =>
{
    await booksService.CreateAsync(newBook);

    return Results.Created($"/books/{newBook.Id}", newBook);
});

booksGroup.MapPut("/{id:length(24)}", async (string id, Book updatedBook, BooksService booksService) =>
{
    var book = await booksService.GetAsync(id);

    if (book is null)
    {
        return Results.NotFound();
    }

    updatedBook.Id = book.Id;

    await booksService.UpdateAsync(id, updatedBook);

    return Results.NoContent();
});

booksGroup.MapDelete("/{id:length(24)}", async (string id, BooksService booksService) =>
{
    var book = await booksService.GetAsync(id);

    if (book is null)
    {
        return Results.NotFound();
    }

    await booksService.RemoveAsync(id);

    return Results.NoContent();
});

await app.RunAsync().ConfigureAwait(false);