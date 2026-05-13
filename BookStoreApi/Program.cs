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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync().ConfigureAwait(false);