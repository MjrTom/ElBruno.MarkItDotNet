using Microsoft.Extensions.AI;

namespace ElBruno.MarkItDotNet.AI.Tests;

/// <summary>
/// A mock <see cref="IChatClient"/> that returns canned responses for testing.
/// </summary>
internal sealed class TestChatClient : IChatClient
{
    private readonly string _cannedResponse;

    public TestChatClient(string cannedResponse = "Mock AI response")
    {
        _cannedResponse = cannedResponse;
    }

    public ChatClientMetadata Metadata { get; } = new("TestChatClient");

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var responseMessage = new ChatMessage(ChatRole.Assistant, _cannedResponse);
        var response = new ChatResponse(responseMessage);
        return Task.FromResult(response);
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Streaming is not supported by TestChatClient.");
    }

    public object? GetService(Type serviceType, object? serviceKey = null) => null;

    public void Dispose() { }
}
