using OSRSData.App.DTOs;

namespace OSRSData.App.Services;

public interface IReportingService
{
    Task<IEnumerable<PlayerLootValueDto>> GetPlayerLootLeaderboardAsync(DateTimeOffset from, DateTimeOffset to);
    Task<IEnumerable<TeamLootValueDto>> GetTeamLootLeaderboardAsync(DateTimeOffset from, DateTimeOffset to);
}
