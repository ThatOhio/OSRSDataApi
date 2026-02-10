using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using OSRSData.App.Services;
using OSRSData.Core.Entities;
using OSRSData.DAL;
using Xunit;

namespace OSRSData.Tests;

public class BingoServiceTests
{
    [Fact]
    public async Task GetBingoConfigAsync_WithMatchingCharacter_ReturnsWebhooksAndItems()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: "BingoTest_Matching")
            .Options;
        
        using var context = new OSRSDbContext(options);
        
        // Seed data
        context.BingoWebhooks.Add(new BingoWebhook 
        { 
            Id = Guid.NewGuid(),
            CharacterName = "TestChar",
            WebhookUrl = "https://discord.com/webhook1",
            CreatedAt = DateTimeOffset.UtcNow
        });
        context.BingoItems.Add(new BingoItem
        {
            Id = Guid.NewGuid(),
            ItemName = "Twisted bow",
            Source = "Chamber of Xeric",
            CreatedAt = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();
        
        var service = new BingoService(context, NullLogger<BingoService>.Instance);
        
        // Act
        var result = await service.GetBingoConfigAsync("TestChar");
        
        // Assert
        Assert.Single(result.Webhooks);
        Assert.Equal("https://discord.com/webhook1", result.Webhooks[0]);
        Assert.Single(result.Items);
        Assert.Equal("Twisted bow", result.Items[0].Name);
        Assert.Equal("Chamber of Xeric", result.Items[0].Source);
    }

    [Fact]
    public async Task GetBingoConfigAsync_WithNoMatchingCharacter_ReturnsEmptyWebhooks()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: "BingoTest_NoMatching")
            .Options;
        
        using var context = new OSRSDbContext(options);
        
        // Seed webhooks for "OtherChar"
        context.BingoWebhooks.Add(new BingoWebhook 
        { 
            Id = Guid.NewGuid(),
            CharacterName = "OtherChar",
            WebhookUrl = "https://discord.com/webhook-other",
            CreatedAt = DateTimeOffset.UtcNow
        });
        
        // Seed a global item
        context.BingoItems.Add(new BingoItem
        {
            Id = Guid.NewGuid(),
            ItemName = "Scimitar",
            Source = null,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();
        
        var service = new BingoService(context, NullLogger<BingoService>.Instance);
        
        // Act
        var result = await service.GetBingoConfigAsync("TestChar");
        
        // Assert
        Assert.Empty(result.Webhooks);
        Assert.Single(result.Items);
        Assert.Equal("Scimitar", result.Items[0].Name);
    }

    [Fact]
    public async Task GetBingoConfigAsync_CaseInsensitiveCharacterMatch_ReturnsWebhooks()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: "BingoTest_CaseInsensitive")
            .Options;
        
        using var context = new OSRSDbContext(options);
        
        context.BingoWebhooks.Add(new BingoWebhook 
        { 
            Id = Guid.NewGuid(),
            CharacterName = "PlayerOne",
            WebhookUrl = "https://discord.com/p1",
            CreatedAt = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();
        
        var service = new BingoService(context, NullLogger<BingoService>.Instance);
        
        // Act
        var result = await service.GetBingoConfigAsync("playerone");
        
        // Assert
        Assert.Single(result.Webhooks);
        Assert.Equal("https://discord.com/p1", result.Webhooks[0]);
    }

    [Fact]
    public async Task GetBingoConfigAsync_ItemsWithNullAndNonNullSources_ReturnedCorrectly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: "BingoTest_Items")
            .Options;
        
        using var context = new OSRSDbContext(options);
        
        context.BingoItems.Add(new BingoItem
        {
            Id = Guid.NewGuid(),
            ItemName = "Item1",
            Source = "Source1",
            CreatedAt = DateTimeOffset.UtcNow
        });
        context.BingoItems.Add(new BingoItem
        {
            Id = Guid.NewGuid(),
            ItemName = "Item2",
            Source = null,
            CreatedAt = DateTimeOffset.UtcNow
        });
        context.BingoItems.Add(new BingoItem
        {
            Id = Guid.NewGuid(),
            ItemName = "Item3",
            Source = "",
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(1)
        });
        await context.SaveChangesAsync();
        
        var service = new BingoService(context, NullLogger<BingoService>.Instance);
        
        // Act
        var result = await service.GetBingoConfigAsync("AnyChar");
        
        // Assert
        Assert.Equal(3, result.Items.Count);
        Assert.Equal("Source1", result.Items[0].Source);
        Assert.Null(result.Items[1].Source);
        Assert.Equal("", result.Items[2].Source);
    }

    [Fact]
    public async Task GetBingoConfigAsync_WithTeamConfig_ReturnsTeamConfig()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: "BingoTest_WithTeamConfig")
            .Options;
        
        using var context = new OSRSDbContext(options);
        
        context.BingoTeamConfigs.Add(new BingoTeamConfig
        {
            Id = Guid.NewGuid(),
            CharacterName = "TestChar",
            TeamName = "Team Alpha",
            TeamNameColor = "#FF5733",
            DateTimeColor = "#1A2B3C",
            CreatedAt = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();
        
        var service = new BingoService(context, NullLogger<BingoService>.Instance);
        
        // Act
        var result = await service.GetBingoConfigAsync("TestChar");
        
        // Assert
        Assert.NotNull(result.TeamConfig);
        Assert.Equal("Team Alpha", result.TeamConfig.TeamName);
        Assert.Equal("#FF5733", result.TeamConfig.TeamNameColor);
        Assert.Equal("#1A2B3C", result.TeamConfig.DateTimeColor);
    }

    [Fact]
    public async Task GetBingoConfigAsync_WithoutTeamConfig_ReturnsNullTeamConfig()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: "BingoTest_WithoutTeamConfig")
            .Options;
        
        using var context = new OSRSDbContext(options);
        
        context.BingoTeamConfigs.Add(new BingoTeamConfig
        {
            Id = Guid.NewGuid(),
            CharacterName = "OtherChar",
            TeamName = "Team Beta",
            TeamNameColor = "#FFFFFF",
            DateTimeColor = "#000000",
            CreatedAt = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();
        
        var service = new BingoService(context, NullLogger<BingoService>.Instance);
        
        // Act
        var result = await service.GetBingoConfigAsync("TestChar");
        
        // Assert
        Assert.Null(result.TeamConfig);
        Assert.Empty(result.Webhooks);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetBingoConfigAsync_TeamConfigCaseInsensitive_ReturnsTeamConfig()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: "BingoTest_TeamConfigCaseInsensitive")
            .Options;
        
        using var context = new OSRSDbContext(options);
        
        context.BingoTeamConfigs.Add(new BingoTeamConfig
        {
            Id = Guid.NewGuid(),
            CharacterName = "PlayerOne",
            TeamName = "Team Gamma",
            TeamNameColor = "#112233",
            DateTimeColor = "#445566",
            CreatedAt = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();
        
        var service = new BingoService(context, NullLogger<BingoService>.Instance);
        
        // Act
        var result = await service.GetBingoConfigAsync("playerone");
        
        // Assert
        Assert.NotNull(result.TeamConfig);
        Assert.Equal("Team Gamma", result.TeamConfig.TeamName);
    }
}
