using System.Text.Json.Serialization;

namespace OSRSData.App.DTOs;

public class BingoTeamIconDto
{
    [JsonPropertyName("teamName")]
    public string TeamName { get; set; } = string.Empty;

    [JsonPropertyName("teamIcon")]
    public string TeamIcon { get; set; } = string.Empty;
}
