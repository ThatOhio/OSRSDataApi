using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using OSRSData.App.Services;
using OSRSData.Core.Entities;
using OSRSData.DAL;
using Xunit;

namespace OSRSData.Tests;

public class BingoTeamsMappingTests
{
    [Fact]
    public async Task GetAllTeamMappingsAsync_NormalisesSpacesToNbsp()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OSRSDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new OSRSDbContext(options);

        context.BingoTeamConfigs.Add(new BingoTeamConfig
        {
            Id = Guid.NewGuid(),
            CharacterName = "Player One",
            TeamName = "TeamX",
            CreatedAt = DateTimeOffset.UtcNow
        });

        await context.SaveChangesAsync();

        var service = new BingoService(context, NullLogger<BingoService>.Instance);

        // Act
        var result = await service.GetAllTeamMappingsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Player\u00a0One", result[0].Character);
        Assert.Equal("TeamX", result[0].TeamName);
    }
}
