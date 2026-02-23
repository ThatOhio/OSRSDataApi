using System.Text.Json.Serialization;

namespace OSRSData.App.DTOs;

public class BingoConfigCloneDto
{
    [JsonPropertyName("sourceCharacterName")]
    public string SourceCharacterName { get; set; } = string.Empty;

    [JsonPropertyName("targetCharacterName")]
    public string TargetCharacterName { get; set; } = string.Empty;
}
