using System.Text.Json.Serialization;

namespace OSRSData.App.DTOs;

public class BingoTeamMappingDto
{
    [JsonPropertyName("character")]
    public string Character { get; set; } = string.Empty;

    [JsonPropertyName("teamName")]
    public string TeamName { get; set; } = string.Empty;
}
