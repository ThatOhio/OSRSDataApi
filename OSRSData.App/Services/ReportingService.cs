using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OSRSData.App.DTOs;
using OSRSData.Core.Entities;
using OSRSData.Core.Enums;
using OSRSData.DAL;

namespace OSRSData.App.Services;

public class ReportingService : IReportingService
{
    private readonly OSRSDbContext _context;
    private readonly ILogger<ReportingService> _logger;

    public ReportingService(OSRSDbContext context, ILogger<ReportingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<PlayerLootValueDto>> GetPlayerLootLeaderboardAsync(DateTimeOffset from, DateTimeOffset to)
    {
        try
        {
            var entries = await GetRelevantLogEntriesAsync(from, to);
            
            // OSRS character names are case-insensitive, using OrdinalIgnoreCase for the dictionary.
            var teamConfigs = await _context.BingoTeamConfigs
                .ToDictionaryAsync(tc => tc.CharacterName, tc => tc.TeamName, StringComparer.OrdinalIgnoreCase);

            var leaderboard = entries
                .GroupBy(e => e.Player ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .Select(g => new PlayerLootValueDto
                {
                    CharacterName = g.Key,
                    TeamName = teamConfigs.TryGetValue(g.Key, out var teamName) ? teamName : string.Empty,
                    TotalLootValue = CalculateDeduplicatedLoot(g)
                })
                .OrderByDescending(p => p.TotalLootValue)
                .ToList();

            return leaderboard;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating player loot leaderboard data");
            throw;
        }
    }

    public async Task<IEnumerable<TeamLootValueDto>> GetTeamLootLeaderboardAsync(DateTimeOffset from, DateTimeOffset to)
    {
        try
        {
            var playerLeaderboard = await GetPlayerLootLeaderboardAsync(from, to);

            var teamLeaderboard = playerLeaderboard
                .GroupBy(p => p.TeamName)
                .Select(g => new TeamLootValueDto
                {
                    TeamName = g.Key,
                    TotalLootValue = g.Sum(p => p.TotalLootValue)
                })
                .OrderByDescending(t => t.TotalLootValue)
                .ToList();

            return teamLeaderboard;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating team loot leaderboard data");
            throw;
        }
    }

    private async Task<List<LogEntry>> GetRelevantLogEntriesAsync(DateTimeOffset from, DateTimeOffset to)
    {
        return await _context.LogEntries
            .AsNoTracking()
            .Include(e => e.LootRecord)
                .ThenInclude(r => r!.Items)
            .Where(e => e.Timestamp >= from && e.Timestamp <= to &&
                        (e.Type == LogType.LOOT || e.Type == LogType.RAID_LOOT || e.Type == LogType.VALUABLE_DROP))
            .ToListAsync();
    }

    private long CalculateDeduplicatedLoot(IEnumerable<LogEntry> playerEntries)
    {
        // Flatten to a list of items, each tagged with their source LogType.
        var allItems = playerEntries
            .Where(e => e.LootRecord != null)
            .SelectMany(e => e.LootRecord!.Items.Select(i => new { e.Type, i }))
            .ToList();

        // Primary pass: Group items by (Name, Quantity) and pick the best one (highest priority log type).
        var bestItems = allItems
            .GroupBy(x => new { x.i.Name, x.i.Quantity })
            .Select(g => g.OrderBy(x => GetLogTypePriority(x.Type)).First())
            .ToList();

        // Secondary pass: Handle cases where the same item has a slightly different name across log types.
        // A VALUABLE_DROP item is a duplicate if there exists a non-VALUABLE_DROP item 
        // whose Name starts with the VALUABLE_DROP item's Name, with the same Quantity.
        var nonValuableDrops = bestItems.Where(x => x.Type != LogType.VALUABLE_DROP).ToList();
        var valuableDrops = bestItems.Where(x => x.Type == LogType.VALUABLE_DROP).ToList();

        long totalValue = 0;

        // Sum all surviving non-VALUABLE_DROP items.
        foreach (var item in nonValuableDrops)
        {
            totalValue += (long)item.i.Price * item.i.Quantity;
        }

        // Sum VALUABLE_DROP items only if they are not duplicates of non-VALUABLE_DROP items.
        foreach (var vd in valuableDrops)
        {
            bool isDuplicate = nonValuableDrops.Any(nvd => 
                nvd.i.Quantity == vd.i.Quantity && 
                nvd.i.Name.StartsWith(vd.i.Name, StringComparison.Ordinal));
            
            if (!isDuplicate)
            {
                totalValue += (long)vd.i.Price * vd.i.Quantity;
            }
        }

        return totalValue;
    }

    private int GetLogTypePriority(LogType type)
    {
        return type switch
        {
            LogType.RAID_LOOT => 1,
            LogType.LOOT => 2,
            LogType.VALUABLE_DROP => 3,
            _ => 99
        };
    }
}
