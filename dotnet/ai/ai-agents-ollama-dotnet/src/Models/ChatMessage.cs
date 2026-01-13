namespace AIAgentsOllama.Models;

/// <summary>
/// Represents a message in the conversation history.
/// </summary>
public class ChatMessage
{
    public ChatMessage(string role, string content)
    {
        Role = role;
        Content = content;
    }

    /// <summary>
    /// Role of the message sender: "user", "assistant", or "tool".
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// Content of the message.
    /// </summary>
    public string Content { get; set; }
}
