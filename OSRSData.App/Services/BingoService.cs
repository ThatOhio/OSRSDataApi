using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OSRSData.App.DTOs;
using OSRSData.Core.Entities;
using OSRSData.DAL;

namespace OSRSData.App.Services;

public class BingoService : IBingoService
{
    private readonly OSRSDbContext _context;
    private readonly ILogger<BingoService> _logger;

    public BingoService(OSRSDbContext context, ILogger<BingoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BingoConfigResponseDto> GetBingoConfigAsync(string characterName)
    {
        try
        {
            var characterNameLower = characterName.ToLower();
            
            var webhooks = await _context.BingoWebhooks
                .Where(w => w.CharacterName.ToLower() == characterNameLower)
                .Select(w => w.WebhookUrl)
                .ToListAsync();

            var items = await _context.BingoItems
                .OrderBy(i => i.CreatedAt)
                .Select(i => new BingoItemDto
                {
                    Name = i.ItemName,
                    Source = i.Source
                })
                .ToListAsync();

            var teamConfigEntity = await _context.BingoTeamConfigs
                .FirstOrDefaultAsync(tc => tc.CharacterName.ToLower() == characterNameLower);

            var teamConfigDto = teamConfigEntity != null
                ? new BingoTeamConfigDto
                {
                    TeamName = teamConfigEntity.TeamName,
                    TeamNameColor = teamConfigEntity.TeamNameColor,
                    DateTimeColor = teamConfigEntity.DateTimeColor
                }
                : null;

            return new BingoConfigResponseDto
            {
                Webhooks = webhooks,
                Items = items,
                TeamConfig = teamConfigDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching bingo config for character {Character}", characterName);
            throw;
        }
    }

    public async Task UpdateBingoTeamConfigAsync(string characterName, BingoTeamConfigUpdateDto updateDto)
    {
        try
        {
            var characterNameLower = characterName.ToLower();
            var teamConfig = await _context.BingoTeamConfigs
                .FirstOrDefaultAsync(tc => tc.CharacterName.ToLower() == characterNameLower);

            if (teamConfig == null)
            {
                teamConfig = new BingoTeamConfig
                {
                    Id = Guid.NewGuid(),
                    CharacterName = characterName,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                _context.BingoTeamConfigs.Add(teamConfig);
            }

            teamConfig.TeamName = updateDto.TeamName;
            teamConfig.TeamNameColor = updateDto.TeamNameColor;
            teamConfig.DateTimeColor = updateDto.DateTimeColor;
            teamConfig.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating bingo team config for character {Character}", characterName);
            throw;
        }
    }
    
    public async Task UpdateBingoTeamConfigsBulkAsync(List<BingoTeamConfigBulkDto> configs)
    {
        try
        {
            var characterNames = configs.Select(c => c.CharacterName.ToLower()).ToList();
            var existingConfigs = await _context.BingoTeamConfigs
                .Where(tc => characterNames.Contains(tc.CharacterName.ToLower()))
                .ToListAsync();

            foreach (var configDto in configs)
            {
                var characterNameLower = configDto.CharacterName.ToLower();
                var teamConfig = existingConfigs
                    .FirstOrDefault(tc => tc.CharacterName.ToLower() == characterNameLower);

                if (teamConfig == null)
                {
                    teamConfig = new BingoTeamConfig
                    {
                        Id = Guid.NewGuid(),
                        CharacterName = configDto.CharacterName,
                        CreatedAt = DateTimeOffset.UtcNow
                    };
                    _context.BingoTeamConfigs.Add(teamConfig);
                }

                teamConfig.TeamName = configDto.TeamName;
                teamConfig.TeamNameColor = configDto.TeamNameColor;
                teamConfig.DateTimeColor = configDto.DateTimeColor;
                teamConfig.UpdatedAt = DateTimeOffset.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk updating bingo team configs");
            throw;
        }
    }

    public async Task AddBingoItemsAsync(List<BingoItemDto> items)
    {
        try
        {
            foreach (var itemDto in items)
            {
                var itemNameLower = itemDto.Name.ToLower();
                var existingItem = await _context.BingoItems
                    .FirstOrDefaultAsync(i => i.ItemName.ToLower() == itemNameLower);

                if (existingItem != null)
                {
                    existingItem.Source = itemDto.Source;
                }
                else
                {
                    _context.BingoItems.Add(new BingoItem
                    {
                        Id = Guid.NewGuid(),
                        ItemName = itemDto.Name,
                        Source = itemDto.Source,
                        CreatedAt = DateTimeOffset.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding bingo items");
            throw;
        }
    }

    public async Task AddBingoWebhookAsync(BingoWebhookUpdateDto webhookDto)
    {
        try
        {
            _context.BingoWebhooks.Add(new BingoWebhook
            {
                Id = Guid.NewGuid(),
                CharacterName = webhookDto.CharacterName,
                WebhookUrl = webhookDto.WebhookUrl,
                CreatedAt = DateTimeOffset.UtcNow
            });

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding bingo webhook for {Character}", webhookDto.CharacterName);
            throw;
        }
    }

    public async Task AddBingoWebhooksBulkAsync(List<BingoWebhookUpdateDto> webhooks)
    {
        try
        {
            foreach (var webhookDto in webhooks)
            {
                _context.BingoWebhooks.Add(new BingoWebhook
                {
                    Id = Guid.NewGuid(),
                    CharacterName = webhookDto.CharacterName,
                    WebhookUrl = webhookDto.WebhookUrl,
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk adding bingo webhooks");
            throw;
        }
    }
}
