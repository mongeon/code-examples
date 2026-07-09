using System.Net.Http.Json;

public class BooksClient(HttpClient httpClient)
{
    public Task<List<Book>?> GetAllAsync() =>
        httpClient.GetFromJsonAsync<List<Book>>("/books");

    public Task<List<Book>?> SearchAsync(string query) =>
        httpClient.GetFromJsonAsync<List<Book>>(
            $"/books/search?q={Uri.EscapeDataString(query)}");

    public async Task<Book?> AddAsync(string title, string author, string status)
    {
        var response = await httpClient.PostAsJsonAsync("/books",
            new Book(0, title, author, status));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Book>();
    }
}

public record Book(int Id, string Title, string Author, string Status);
