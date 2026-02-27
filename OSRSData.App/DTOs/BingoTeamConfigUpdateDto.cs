using System.Text.Json.Serialization;

namespace OSRSData.App.DTOs;

public class BingoTeamConfigUpdateDto
{
    [JsonPropertyName("teamName")]
    public string TeamName { get; set; } = string.Empty;

    [JsonPropertyName("teamNameColor")]
    public string TeamNameColor { get; set; } = string.Empty;

    [JsonPropertyName("dateTimeColor")]
    public string DateTimeColor { get; set; } = string.Empty;

    [JsonPropertyName("teamIcon")]
    public string TeamIcon { get; set; } = string.Empty;
}
