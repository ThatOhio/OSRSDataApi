using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using OSRSData.App.DTOs;
using OSRSData.App.Services;
using OSRSData.Core.Entities;
using OSRSData.Core.Enums;
using OSRSData.DAL;
using Xunit;

namespace OSRSData.Tests;

public class LogServiceTests
{
    [Fact]
    public async Task ProcessDeathRecordAsync_ShouldSaveDeathRecord()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        using var context = new OSRSDbContext(options);
        var service = new LogService(context, NullLogger<LogService>.Instance);

        var deathEntry = new DeathEntryDto
        {
            Player = "TestPlayer",
            Type = "DEATH",
            Timestamp = 1705354920000,
            Data = new DeathRecordDto
            {
                RegionId = 12345,
                Killer = "TestKiller"
            }
        };

        // Act
        await service.ProcessDeathRecordAsync(deathEntry, "127.0.0.1", "TestAgent");

        // Assert
        var entry = await context.LogEntries
            .Include(e => e.DeathRecord)
            .FirstOrDefaultAsync(e => e.Player == "TestPlayer");

        Assert.NotNull(entry);
        Assert.Equal(LogType.DEATH, entry.Type);
        Assert.NotNull(entry.DeathRecord);
        Assert.Equal(12345, entry.DeathRecord.RegionId);
        Assert.Equal("TestKiller", entry.DeathRecord.Killer);
        Assert.Equal("127.0.0.1", entry.IpAddress);
        Assert.Equal("TestAgent", entry.UserAgent);
    }

    [Fact]
    public async Task ProcessLogsAsync_ShouldSaveLootLogs()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDbLoot")
            .Options;

        using var context = new OSRSDbContext(options);
        var service = new LogService(context, NullLogger<LogService>.Instance);

        var logs = new List<LogEntryDto>
        {
            new LogEntryDto
            {
                Player = "LootPlayer",
                Type = "LOOT",
                Timestamp = 1705354920000,
                Data = new LootRecordDto
                {
                    Source = "Test Boss",
                    Items = new List<LootItemDto>
                    {
                        new LootItemDto { Id = 1, Name = "Item 1", Quantity = 1, Price = 100 }
                    },
                    TotalValue = 100,
                    Kc = 1
                }
            }
        };

        // Act
        await service.ProcessLogsAsync(logs, "127.0.0.1", "TestAgent");

        // Assert
        var entry = await context.LogEntries
            .Include(e => e.LootRecord)
            .ThenInclude(r => r!.Items)
            .FirstOrDefaultAsync(e => e.Player == "LootPlayer");

        Assert.NotNull(entry);
        Assert.Equal(LogType.LOOT, entry.Type);
        Assert.NotNull(entry.LootRecord);
        Assert.Equal("Test Boss", entry.LootRecord.Source);
        Assert.Single(entry.LootRecord.Items);
        Assert.Equal("Item 1", entry.LootRecord.Items.First().Name);
    }
}
