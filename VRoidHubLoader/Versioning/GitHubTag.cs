namespace CustomAvatarLoader.Versioning;

using System.Text.Json.Serialization;

public class GitHubTag
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}