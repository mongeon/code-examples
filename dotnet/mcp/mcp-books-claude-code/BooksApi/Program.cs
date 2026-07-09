var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var books = new List<Book>
{
    new(1, "Clean Code", "Robert C. Martin", "done"),
    new(2, "The Pragmatic Programmer", "Hunt and Thomas", "reading"),
    new(3, "Working Effectively with Legacy Code", "Michael Feathers", "to-read")
};

app.MapGet("/books", () => books);

app.MapGet("/books/search", (string q) =>
    books.Where(b => b.Title.Contains(q, StringComparison.OrdinalIgnoreCase)
                  || b.Author.Contains(q, StringComparison.OrdinalIgnoreCase)));

app.MapPost("/books", (Book book) =>
{
    var created = book with { Id = books.Max(b => b.Id) + 1 };
    books.Add(created);
    return Results.Created($"/books/{created.Id}", created);
});

app.Run("http://localhost:5200");

public record Book(int Id, string Title, string Author, string Status);
