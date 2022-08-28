using System.Text.Json.Serialization;

namespace TwitterStreamV2App.Models;

public class TwitterStreamResponse
{
    [JsonPropertyName("entities")]
    public Entities? Entities { get; set; }
}

public class Entities
{
    [JsonPropertyName("hashtags")]
    public List<Hashtag>? Hashtags { get; set; }
}

public class Hashtag
{
    [JsonPropertyName("tag")]
    public string? Tag { get; set; }
}