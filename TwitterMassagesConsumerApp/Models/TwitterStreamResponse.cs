using System.Text.Json.Serialization;

namespace TwitterMassagesConsumerApp.Models;

public record TwitterStreamResponse
{
    [JsonPropertyName("entities")]
    public Entities Entities { get; set; }
}

public record Entities
{
    [JsonPropertyName("hashtags")]
    public List<Hashtag>? Hashtags { get; set; }
}

public record Hashtag
{
    [JsonPropertyName("tag")]
    public string Tag { get; set; }
}