using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using OSRSData.App.DTOs;
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

    [Fact]
    public async Task AddBingoItemsAsync_NewItems_AddsToDatabase()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new OSRSDbContext(options);
        var service = new BingoService(context, NullLogger<BingoService>.Instance);

        var items = new List<BingoItemDto>
        {
            new() { Name = "Twisted bow", Source = "CoX" },
            new() { Name = "Scythe of vitur", Source = "ToB" }
        };

        // Act
        await service.AddBingoItemsAsync(items);

        // Assert
        Assert.Equal(2, await context.BingoItems.CountAsync());
        var tbow = await context.BingoItems.FirstAsync(i => i.ItemName == "Twisted bow");
        Assert.Equal("CoX", tbow.Source);
    }

    [Fact]
    public async Task AddBingoItemsAsync_ExistingItems_UpdatesSource()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new OSRSDbContext(options);
        context.BingoItems.Add(new BingoItem
        {
            Id = Guid.NewGuid(),
            ItemName = "Twisted bow",
            Source = "Original",
            CreatedAt = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();

        var service = new BingoService(context, NullLogger<BingoService>.Instance);

        var items = new List<BingoItemDto>
        {
            new() { Name = "twisted bow", Source = "Updated" } // Case insensitive match
        };

        // Act
        await service.AddBingoItemsAsync(items);

        // Assert
        Assert.Equal(1, await context.BingoItems.CountAsync());
        var tbow = await context.BingoItems.FirstAsync(i => i.ItemName == "Twisted bow");
        Assert.Equal("Updated", tbow.Source);
    }

    [Fact]
    public async Task AddBingoWebhookAsync_AddsNewWebhook()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new OSRSDbContext(options);
        var service = new BingoService(context, NullLogger<BingoService>.Instance);

        var webhookDto = new BingoWebhookUpdateDto
        {
            CharacterName = "TestPlayer",
            WebhookUrl = "https://example.com/webhook"
        };

        // Act
        await service.AddBingoWebhookAsync(webhookDto);

        // Assert
        Assert.Equal(1, await context.BingoWebhooks.CountAsync());
        var webhook = await context.BingoWebhooks.FirstAsync();
        Assert.Equal("TestPlayer", webhook.CharacterName);
        Assert.Equal("https://example.com/webhook", webhook.WebhookUrl);
    }

    [Fact]
    public async Task UpdateBingoTeamConfigAsync_NewConfig_CreatesConfig()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new OSRSDbContext(options);
        var service = new BingoService(context, NullLogger<BingoService>.Instance);
        var updateDto = new BingoTeamConfigUpdateDto
        {
            TeamName = "New Team",
            TeamNameColor = "#112233",
            DateTimeColor = "#445566"
        };

        // Act
        await service.UpdateBingoTeamConfigAsync("NewPlayer", updateDto);

        // Assert
        var config = await context.BingoTeamConfigs.FirstOrDefaultAsync(tc => tc.CharacterName == "NewPlayer");
        Assert.NotNull(config);
        Assert.Equal("New Team", config.TeamName);
        Assert.Equal("#112233", config.TeamNameColor);
        Assert.Equal("#445566", config.DateTimeColor);
    }

    [Fact]
    public async Task UpdateBingoTeamConfigAsync_ExistingConfig_UpdatesConfig()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new OSRSDbContext(options);
        context.BingoTeamConfigs.Add(new BingoTeamConfig
        {
            Id = Guid.NewGuid(),
            CharacterName = "OldPlayer",
            TeamName = "Old Team",
            TeamNameColor = "#000000",
            DateTimeColor = "#000000",
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        });
        await context.SaveChangesAsync();

        var service = new BingoService(context, NullLogger<BingoService>.Instance);
        var updateDto = new BingoTeamConfigUpdateDto
        {
            TeamName = "Updated Team",
            TeamNameColor = "#FFFFFF",
            DateTimeColor = "#FFFFFF"
        };

        // Act
        await service.UpdateBingoTeamConfigAsync("oldplayer", updateDto);

        // Assert
        var config = await context.BingoTeamConfigs.FirstOrDefaultAsync(tc => tc.CharacterName == "OldPlayer");
        Assert.NotNull(config);
        Assert.Equal("Updated Team", config.TeamName);
        Assert.Equal("#FFFFFF", config.TeamNameColor);
        Assert.Equal("#FFFFFF", config.DateTimeColor);
        Assert.NotNull(config.UpdatedAt);
    }

    [Fact]
    public async Task UpdateBingoTeamConfigsBulkAsync_UpdatesAndInsertsCorrectly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: "BingoTest_BulkUpdate")
            .Options;
        
        using var context = new OSRSDbContext(options);
        
        context.BingoTeamConfigs.Add(new BingoTeamConfig
        {
            Id = Guid.NewGuid(),
            CharacterName = "ExistingChar",
            TeamName = "Old Name",
            TeamNameColor = "#000000",
            DateTimeColor = "#000000",
            CreatedAt = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();
        
        var service = new BingoService(context, NullLogger<BingoService>.Instance);
        
        var configs = new List<BingoTeamConfigBulkDto>
        {
            new BingoTeamConfigBulkDto
            {
                CharacterName = "ExistingChar",
                TeamName = "New Name",
                TeamNameColor = "#111111",
                DateTimeColor = "#222222"
            },
            new BingoTeamConfigBulkDto
            {
                CharacterName = "NewChar",
                TeamName = "Brand New Team",
                TeamNameColor = "#333333",
                DateTimeColor = "#444444"
            }
        };
        
        // Act
        await service.UpdateBingoTeamConfigsBulkAsync(configs);
        
        // Assert
        var existing = await context.BingoTeamConfigs.FirstOrDefaultAsync(tc => tc.CharacterName == "ExistingChar");
        Assert.NotNull(existing);
        Assert.Equal("New Name", existing.TeamName);
        Assert.Equal("#111111", existing.TeamNameColor);
        Assert.NotNull(existing.UpdatedAt);
        
        var newlyAdded = await context.BingoTeamConfigs.FirstOrDefaultAsync(tc => tc.CharacterName == "NewChar");
        Assert.NotNull(newlyAdded);
        Assert.Equal("Brand New Team", newlyAdded.TeamName);
        Assert.Equal("#333333", newlyAdded.TeamNameColor);
        Assert.NotNull(newlyAdded.UpdatedAt);
    }

    [Fact]
    public async Task AddBingoWebhooksBulkAsync_AddsMultipleWebhooks()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new OSRSDbContext(options);
        var service = new BingoService(context, NullLogger<BingoService>.Instance);

        var webhooks = new List<BingoWebhookUpdateDto>
        {
            new BingoWebhookUpdateDto { CharacterName = "Player1", WebhookUrl = "url1" },
            new BingoWebhookUpdateDto { CharacterName = "Player2", WebhookUrl = "url2" }
        };

        // Act
        await service.AddBingoWebhooksBulkAsync(webhooks);

        // Assert
        Assert.Equal(2, await context.BingoWebhooks.CountAsync());
        var allWebhooks = await context.BingoWebhooks.ToListAsync();
        Assert.Contains(allWebhooks, w => w.CharacterName == "Player1" && w.WebhookUrl == "url1");
        Assert.Contains(allWebhooks, w => w.CharacterName == "Player2" && w.WebhookUrl == "url2");
    }

    [Fact]
    public async Task CloneBingoConfigAsync_ClonesTeamConfigAndWebhooks()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new OSRSDbContext(options);
        
        // Setup source character
        context.BingoTeamConfigs.Add(new BingoTeamConfig
        {
            Id = Guid.NewGuid(),
            CharacterName = "SourcePlayer",
            TeamName = "Source Team",
            TeamNameColor = "#111111",
            DateTimeColor = "#222222",
            CreatedAt = DateTimeOffset.UtcNow
        });
        context.BingoWebhooks.Add(new BingoWebhook
        {
            Id = Guid.NewGuid(),
            CharacterName = "SourcePlayer",
            WebhookUrl = "https://source.com/webhook1",
            CreatedAt = DateTimeOffset.UtcNow
        });
        context.BingoWebhooks.Add(new BingoWebhook
        {
            Id = Guid.NewGuid(),
            CharacterName = "SourcePlayer",
            WebhookUrl = "https://source.com/webhook2",
            CreatedAt = DateTimeOffset.UtcNow
        });
        
        // Setup target character with existing config (to test update)
        context.BingoTeamConfigs.Add(new BingoTeamConfig
        {
            Id = Guid.NewGuid(),
            CharacterName = "TargetPlayer",
            TeamName = "Old Team",
            TeamNameColor = "#000000",
            DateTimeColor = "#000000",
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        });
        
        await context.SaveChangesAsync();

        var service = new BingoService(context, NullLogger<BingoService>.Instance);
        var cloneDto = new BingoConfigCloneDto
        {
            SourceCharacterName = "SourcePlayer",
            TargetCharacterName = "TargetPlayer"
        };

        // Act
        await service.CloneBingoConfigAsync(cloneDto);

        // Assert
        // Check Team Config
        var targetTeamConfig = await context.BingoTeamConfigs
            .FirstOrDefaultAsync(tc => tc.CharacterName == "TargetPlayer");
        Assert.NotNull(targetTeamConfig);
        Assert.Equal("Source Team", targetTeamConfig.TeamName);
        Assert.Equal("#111111", targetTeamConfig.TeamNameColor);
        Assert.Equal("#222222", targetTeamConfig.DateTimeColor);
        Assert.NotNull(targetTeamConfig.UpdatedAt);

        // Check Webhooks
        var targetWebhooks = await context.BingoWebhooks
            .Where(w => w.CharacterName == "TargetPlayer")
            .ToListAsync();
        Assert.Equal(2, targetWebhooks.Count);
        Assert.Contains(targetWebhooks, w => w.WebhookUrl == "https://source.com/webhook1");
        Assert.Contains(targetWebhooks, w => w.WebhookUrl == "https://source.com/webhook2");
    }

    [Fact]
    public async Task CloneBingoConfigsBulkAsync_ClonesMultipleConfigurations()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new OSRSDbContext(options);
        
        // Seed Source 1
        context.BingoTeamConfigs.Add(new BingoTeamConfig
        {
            Id = Guid.NewGuid(),
            CharacterName = "Source1",
            TeamName = "Team1",
            TeamNameColor = "#111111",
            DateTimeColor = "#111111",
            CreatedAt = DateTimeOffset.UtcNow
        });
        context.BingoWebhooks.Add(new BingoWebhook
        {
            Id = Guid.NewGuid(),
            CharacterName = "Source1",
            WebhookUrl = "https://source1.com/w1",
            CreatedAt = DateTimeOffset.UtcNow
        });

        // Seed Source 2
        context.BingoTeamConfigs.Add(new BingoTeamConfig
        {
            Id = Guid.NewGuid(),
            CharacterName = "Source2",
            TeamName = "Team2",
            TeamNameColor = "#222222",
            DateTimeColor = "#222222",
            CreatedAt = DateTimeOffset.UtcNow
        });

        await context.SaveChangesAsync();
        var service = new BingoService(context, NullLogger<BingoService>.Instance);
        
        var cloneDtos = new List<BingoConfigCloneDto>
        {
            new BingoConfigCloneDto { SourceCharacterName = "Source1", TargetCharacterName = "Target1" },
            new BingoConfigCloneDto { SourceCharacterName = "Source2", TargetCharacterName = "Target2" }
        };

        // Act
        await service.CloneBingoConfigsBulkAsync(cloneDtos);

        // Assert
        // Check Target 1
        var target1Config = await context.BingoTeamConfigs.FirstOrDefaultAsync(tc => tc.CharacterName == "Target1");
        Assert.NotNull(target1Config);
        Assert.Equal("Team1", target1Config.TeamName);
        var target1Webhooks = await context.BingoWebhooks.Where(w => w.CharacterName == "Target1").ToListAsync();
        Assert.Single(target1Webhooks);
        Assert.Equal("https://source1.com/w1", target1Webhooks[0].WebhookUrl);

        // Check Target 2
        var target2Config = await context.BingoTeamConfigs.FirstOrDefaultAsync(tc => tc.CharacterName == "Target2");
        Assert.NotNull(target2Config);
        Assert.Equal("Team2", target2Config.TeamName);
        var target2Webhooks = await context.BingoWebhooks.Where(w => w.CharacterName == "Target2").ToListAsync();
        Assert.Empty(target2Webhooks);
    }

    [Fact]
    public async Task CloneBingoConfigsBulkAsync_ChainedClones_ClonesTeamConfigsCorrectly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new OSRSDbContext(options);
        
        // Seed Source A
        context.BingoTeamConfigs.Add(new BingoTeamConfig
        {
            Id = Guid.NewGuid(),
            CharacterName = "A",
            TeamName = "TeamA",
            TeamNameColor = "#AAAAAA",
            DateTimeColor = "#AAAAAA",
            CreatedAt = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();

        var service = new BingoService(context, NullLogger<BingoService>.Instance);
        
        var cloneDtos = new List<BingoConfigCloneDto>
        {
            new BingoConfigCloneDto { SourceCharacterName = "A", TargetCharacterName = "B" },
            new BingoConfigCloneDto { SourceCharacterName = "B", TargetCharacterName = "C" }
        };

        // Act
        await service.CloneBingoConfigsBulkAsync(cloneDtos);

        // Assert
        var configB = await context.BingoTeamConfigs.FirstOrDefaultAsync(tc => tc.CharacterName == "B");
        var configC = await context.BingoTeamConfigs.FirstOrDefaultAsync(tc => tc.CharacterName == "C");
        Assert.NotNull(configB);
        Assert.Equal("TeamA", configB.TeamName);
        Assert.NotNull(configC);
        Assert.Equal("TeamA", configC.TeamName);
    }
}
