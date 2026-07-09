using System.ComponentModel;
using ModelContextProtocol.Server;

[McpServerToolType]
public static class BooksTools
{
    [McpServerTool]
    [Description("Lists every book in the personal reading tracker, with author and status.")]
    public static async Task<string> ListBooks(BooksClient client)
    {
        var books = await client.GetAllAsync();
        if (books is null || books.Count == 0)
            return "The reading list is empty.";

        return string.Join("\n",
            books.Select(b => $"- {b.Title} ({b.Author}): {b.Status}"));
    }

    [McpServerTool]
    [Description("Searches the reading tracker by book title or author name.")]
    public static async Task<string> SearchBooks(
        BooksClient client,
        [Description("Text to look for in the title or author (e.g. 'legacy', 'Martin')")]
        string query)
    {
        var books = await client.SearchAsync(query);
        if (books is null || books.Count == 0)
            return $"No book matches '{query}'.";

        return string.Join("\n",
            books.Select(b => $"- {b.Title} ({b.Author}): {b.Status}"));
    }

    [McpServerTool]
    [Description("Adds a book to the personal reading tracker.")]
    public static async Task<string> AddBook(
        BooksClient client,
        [Description("Book title")] string title,
        [Description("Author name")] string author,
        [Description("Reading status: 'to-read', 'reading' or 'done'")] string status)
    {
        var book = await client.AddAsync(title, author, status);
        if (book is null)
            return "The book could not be added.";

        return $"Added '{book.Title}' by {book.Author} with status '{book.Status}'.";
    }
}
