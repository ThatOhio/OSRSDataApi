using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using OSRSData.App.DTOs;
using OSRSData.App.Services;
using OSRSData.Core.Entities;
using OSRSData.Core.Enums;
using OSRSData.DAL;
using Xunit;

namespace OSRSData.Tests;

public class ReportingServiceTests
{
    private OSRSDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new OSRSDbContext(options);
    }

    [Fact]
    public async Task GetPlayerLootLeaderboardAsync_DeduplicatesCorrectly()
    {
        // Arrange
        var context = GetDbContext();
        var service = new ReportingService(context, NullLogger<ReportingService>.Instance);

        var from = DateTimeOffset.UtcNow.AddDays(-1);
        var to = DateTimeOffset.UtcNow.AddDays(1);

        // Player 1 has duplicates across LOOT and VALUABLE_DROP
        var p1 = "Player1";
        context.LogEntries.Add(new LogEntry
        {
            Id = Guid.NewGuid(),
            Player = p1,
            Type = LogType.LOOT,
            Timestamp = DateTimeOffset.UtcNow,
            LootRecord = new LootRecord
            {
                Id = Guid.NewGuid(),
                Source = "Source1",
                Items = new List<LootItem>
                {
                    new LootItem { Id = Guid.NewGuid(), Name = "Item1", Quantity = 1, Price = 1000 },
                    new LootItem { Id = Guid.NewGuid(), Name = "Item2", Quantity = 2, Price = 500 }
                }
            }
        });

        context.LogEntries.Add(new LogEntry
        {
            Id = Guid.NewGuid(),
            Player = p1,
            Type = LogType.VALUABLE_DROP,
            Timestamp = DateTimeOffset.UtcNow,
            LootRecord = new LootRecord
            {
                Id = Guid.NewGuid(),
                Source = "Source1",
                Items = new List<LootItem>
                {
                    new LootItem { Id = Guid.NewGuid(), Name = "Item1", Quantity = 1, Price = 1000 }
                }
            }
        });

        // Player 2 has RAID_LOOT
        var p2 = "Player2";
        context.LogEntries.Add(new LogEntry
        {
            Id = Guid.NewGuid(),
            Player = p2,
            Type = LogType.RAID_LOOT,
            Timestamp = DateTimeOffset.UtcNow,
            LootRecord = new LootRecord
            {
                Id = Guid.NewGuid(),
                Source = "Raid",
                Items = new List<LootItem>
                {
                    new LootItem { Id = Guid.NewGuid(), Name = "Item3", Quantity = 1, Price = 5000 }
                }
            }
        });

        // Team config for Player 1
        context.BingoTeamConfigs.Add(new BingoTeamConfig
        {
            Id = Guid.NewGuid(),
            CharacterName = p1,
            TeamName = "TeamA",
            CreatedAt = DateTimeOffset.UtcNow
        });

        await context.SaveChangesAsync();

        // Act
        var result = (await service.GetPlayerLootLeaderboardAsync(from, to)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        
        var r1 = result.First(r => r.CharacterName == p1);
        Assert.Equal("TeamA", r1.TeamName);
        // Item1 (1000) + Item2 (2*500) = 2000. VALUABLE_DROP Item1 is deduplicated.
        Assert.Equal(2000, r1.TotalLootValue);

        var r2 = result.First(r => r.CharacterName == p2);
        Assert.Equal("", r2.TeamName);
        Assert.Equal(5000, r2.TotalLootValue);

        // Sorted descending
        Assert.Equal(p2, result[0].CharacterName);
        Assert.Equal(p1, result[1].CharacterName);
    }

    [Fact]
    public async Task GetTeamLootLeaderboardAsync_GroupsCorrectly()
    {
        // Arrange
        var context = GetDbContext();
        var service = new ReportingService(context, NullLogger<ReportingService>.Instance);

        var from = DateTimeOffset.UtcNow.AddDays(-1);
        var to = DateTimeOffset.UtcNow.AddDays(1);

        // Team A
        context.BingoTeamConfigs.Add(new BingoTeamConfig { Id = Guid.NewGuid(), CharacterName = "P1", TeamName = "TeamA" });
        context.BingoTeamConfigs.Add(new BingoTeamConfig { Id = Guid.NewGuid(), CharacterName = "P2", TeamName = "TeamA" });
        
        // Team B
        context.BingoTeamConfigs.Add(new BingoTeamConfig { Id = Guid.NewGuid(), CharacterName = "P3", TeamName = "TeamB" });

        // Logs
        context.LogEntries.Add(CreateLog("P1", LogType.LOOT, "I1", 1, 100));
        context.LogEntries.Add(CreateLog("P2", LogType.LOOT, "I2", 1, 200));
        context.LogEntries.Add(CreateLog("P3", LogType.LOOT, "I3", 1, 500));
        context.LogEntries.Add(CreateLog("P4", LogType.LOOT, "I4", 1, 1000)); // No team

        await context.SaveChangesAsync();

        // Act
        var result = (await service.GetTeamLootLeaderboardAsync(from, to)).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        
        var tNoTeam = result.First(r => r.TeamName == "");
        Assert.Equal(1000, tNoTeam.TotalLootValue);

        var tB = result.First(r => r.TeamName == "TeamB");
        Assert.Equal(500, tB.TotalLootValue);

        var tA = result.First(r => r.TeamName == "TeamA");
        Assert.Equal(300, tA.TotalLootValue);
    }

    private LogEntry CreateLog(string player, LogType type, string itemName, int qty, int price)
    {
        return new LogEntry
        {
            Id = Guid.NewGuid(),
            Player = player,
            Type = type,
            Timestamp = DateTimeOffset.UtcNow,
            LootRecord = new LootRecord
            {
                Id = Guid.NewGuid(),
                Items = new List<LootItem>
                {
                    new LootItem { Id = Guid.NewGuid(), Name = itemName, Quantity = qty, Price = price }
                }
            }
        };
    }
}
