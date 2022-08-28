using System.Text.Json.Serialization;

namespace TwitterStreamV2App.Models;

public class TwitterSingleObject
{
    [JsonPropertyName("errors")]
    public List<Error>? Errors { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
    
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    
    [JsonPropertyName("data")]
    public TwitterStreamResponse? Data { get; set; }
}

public class Error
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}