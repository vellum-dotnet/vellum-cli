using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

public record SseMessage
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; init; } = null!;
    
    [JsonPropertyName("changes")]
    public List<Change> Changes { get; init; } = [];
}

public record Change(string Type, string Path);

public record SseClientId
{
    [JsonPropertyName("clientId")]
    public string ClientId { get; init; } = null!;
}

public record SseClient(HttpResponse Response, CancellationTokenSource Cancel);

public interface ISseHolder
{
    Task AddAsync(HttpContext context);

    Task SendMessageAsync(SseMessage message);
}

public static class SseHolderMapper
{
    public static IApplicationBuilder UseServerSideExtensionsChangeNotifications(this IApplicationBuilder app, PathString path)
    {
        return app.Map(path, (app) => app.UseMiddleware<SseMiddleware>());
    }
}

public class SseMiddleware
{
    private readonly RequestDelegate next;
    private readonly ISseHolder sse;

    public SseMiddleware(RequestDelegate next, ISseHolder sse)
    {
        this.next = next;
        this.sse = sse;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await this.sse.AddAsync(context);
    }
}

public class SseHolder : ISseHolder
{
    private readonly ILogger<SseHolder> logger;
    private readonly ConcurrentDictionary<string, SseClient> clients = new();

    public SseHolder(ILogger<SseHolder> logger, IHostApplicationLifetime applicationLifetime)
    {
        this.logger = logger;
        applicationLifetime.ApplicationStopping.Register(this.OnShutdown);
    }

    public async Task AddAsync(HttpContext context)
    {
        string clientId = this.CreateId();
        CancellationTokenSource cancel = new();
        SseClient client = new(Response: context.Response, Cancel: cancel);
        
        if (this.clients.TryAdd(clientId, client))
        {
            this.HandshakeAsync(clientId, client);
            context.RequestAborted.WaitHandle.WaitOne();
            this.RemoveClient(clientId);

            await Task.FromResult(true);
        }
    }

    public async Task SendMessageAsync(SseMessage message)
    {
        foreach (KeyValuePair<string, SseClient> c in this.clients)
        {
            if (c.Key == message.Id)
            {
                continue;
            }

            string messageJson = JsonSerializer.Serialize(message);
            await c.Value.Response.WriteAsync($"event: refresh\n", c.Value.Cancel.Token);
            await c.Value.Response.WriteAsync($"data: {messageJson}\n\n", c.Value.Cancel.Token);
            await c.Value.Response.Body.FlushAsync(c.Value.Cancel.Token);
        }
    }
    private async void HandshakeAsync(string clientId, SseClient client)
    {
        try
        {
            string clientIdJson = JsonSerializer.Serialize(new SseClientId { ClientId = clientId });

            client.Response.Headers.Append("Content-Type", new("text/event-stream"));
            client.Response.Headers.Append("Cache-Control", new("no-cache"));
            client.Response.Headers.Append("Connection", new("keep-alive"));
            
            // Send ID to client-side after connecting
            await client.Response.WriteAsync($"data: {clientIdJson}\r\r", client.Cancel.Token);
            await client.Response.Body.FlushAsync(client.Cancel.Token);
        }
        catch (OperationCanceledException ex)
        {
            this.logger.LogError($"Exception {ex.Message}");
        }
    }

    private void OnShutdown()
    {
        List<KeyValuePair<string, SseClient>> tmpClients = [];
        
        foreach (KeyValuePair<string, SseClient> c in this.clients)
        {
            c.Value.Cancel.Cancel();
            tmpClients.Add(c);
        }

        foreach (KeyValuePair<string, SseClient> c in tmpClients)
        {
            this.clients.TryRemove(c);
        }
    }
    public void RemoveClient(string id)
    {
        KeyValuePair<string, SseClient> target = this.clients.FirstOrDefault(c => c.Key == id);
        
        if (string.IsNullOrEmpty(target.Key))
        {
            return;
        }

        target.Value.Cancel.Cancel();
        this.clients.TryRemove(target);
    }

    private string CreateId()
    {
        return Guid.NewGuid().ToString();
    }
}