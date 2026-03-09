using System.Text;
using Microsoft.AspNetCore.Mvc;
using OSRSData.App.DTOs;
using OSRSData.App.Services;

namespace OSRSData.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportingController : ControllerBase
{
    private readonly IReportingService _reportingService;
    private readonly ILogger<ReportingController> _logger;

    public ReportingController(IReportingService reportingService, ILogger<ReportingController> logger)
    {
        _reportingService = reportingService;
        _logger = logger;
    }

    [HttpGet("leaderboard/player-loot-value")]
    public async Task<IActionResult> GetPlayerLootLeaderboard([FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to)
    {
        if (from == null || to == null)
        {
            return BadRequest("Both 'from' and 'to' parameters are required.");
        }

        if (from >= to)
        {
            return BadRequest("'from' must be earlier than 'to'.");
        }

        try
        {
            var data = await _reportingService.GetPlayerLootLeaderboardAsync(from.Value, to.Value);
            var csv = BuildPlayerLootCsv(data);
            var bytes = Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "player_loot_leaderboard.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating player loot leaderboard");
            return StatusCode(500, "An error occurred while generating the report.");
        }
    }

    [HttpGet("leaderboard/team-loot-value")]
    public async Task<IActionResult> GetTeamLootLeaderboard([FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to)
    {
        if (from == null || to == null)
        {
            return BadRequest("Both 'from' and 'to' parameters are required.");
        }

        if (from >= to)
        {
            return BadRequest("'from' must be earlier than 'to'.");
        }

        try
        {
            var data = await _reportingService.GetTeamLootLeaderboardAsync(from.Value, to.Value);
            var csv = BuildTeamLootCsv(data);
            var bytes = Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "team_loot_leaderboard.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating team loot leaderboard");
            return StatusCode(500, "An error occurred while generating the report.");
        }
    }

    private string BuildPlayerLootCsv(IEnumerable<PlayerLootValueDto> data)
    {
        var sb = new StringBuilder();
        sb.AppendLine("character_name,team_name,total_loot_value");
        foreach (var item in data)
        {
            sb.AppendLine($"{EscapeCsv(item.CharacterName)},{EscapeCsv(item.TeamName)},{item.TotalLootValue}");
        }
        return sb.ToString();
    }

    private string BuildTeamLootCsv(IEnumerable<TeamLootValueDto> data)
    {
        var sb = new StringBuilder();
        sb.AppendLine("team_name,total_loot_value");
        foreach (var item in data)
        {
            sb.AppendLine($"{EscapeCsv(item.TeamName)},{item.TotalLootValue}");
        }
        return sb.ToString();
    }

    private string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }
}
