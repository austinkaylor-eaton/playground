using Microsoft.EntityFrameworkCore;
using TodoApi.Minimal;
using Constants = TodoApi.Minimal.Constants;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

#region Swagger Middleware

/*
 * Enables the API Explorer, which is a service that provides metadata about the HTTP API.
 * The API Explorer is used by Swagger to generate the Swagger document.
 */
builder.Services.AddEndpointsApiExplorer();
/*
 * Adds the Swagger OpenAPI document generator to the application services and
 * configures it to provide more information about the API, such as its title and version
 *
 * See https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-8.0&viewFallbackFrom=aspnetcore-10.0&tabs=visual-studio#customize-api-documentation
 * for more information
 */
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "TodoAPI";
    config.Title = "TodoAPI v1";
    config.Version = "v1";
});

#endregion

var app = builder.Build();

#region Swagger Middleware

/*
 * Enables the Swagger UI middleware, which provides a web-based interface for exploring and testing the API.
 * The Swagger UI is automatically generated based on the OpenAPI document.
 */
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "TodoAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

#endregion

app.MapGroup($"/{Constants.TodoItems.EndpointGroupTag.ToLowerInvariant()}")
    .MapTodoItemEndpoints();

await app.RunAsync().ConfigureAwait(false);