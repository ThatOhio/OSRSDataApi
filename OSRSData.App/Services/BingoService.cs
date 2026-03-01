using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
                    DateTimeColor = teamConfigEntity.DateTimeColor,
                    TeamIcon = teamConfigEntity.TeamIcon
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
            teamConfig.TeamIcon = updateDto.TeamIcon;
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
                teamConfig.TeamIcon = configDto.TeamIcon;
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

    public async Task CloneBingoConfigAsync(BingoConfigCloneDto cloneDto)
    {
        try
        {
            var sourceNameLower = cloneDto.SourceCharacterName.ToLower();
            var targetNameLower = cloneDto.TargetCharacterName.ToLower();

            // 1. Clone Team Config
            var sourceTeamConfig = await _context.BingoTeamConfigs
                .FirstOrDefaultAsync(tc => tc.CharacterName.ToLower() == sourceNameLower);

            if (sourceTeamConfig != null)
            {
                var targetTeamConfig = await _context.BingoTeamConfigs
                    .FirstOrDefaultAsync(tc => tc.CharacterName.ToLower() == targetNameLower);

                if (targetTeamConfig == null)
                {
                    targetTeamConfig = new BingoTeamConfig
                    {
                        Id = Guid.NewGuid(),
                        CharacterName = cloneDto.TargetCharacterName,
                        CreatedAt = DateTimeOffset.UtcNow
                    };
                    _context.BingoTeamConfigs.Add(targetTeamConfig);
                }

                targetTeamConfig.TeamName = sourceTeamConfig.TeamName;
                targetTeamConfig.TeamNameColor = sourceTeamConfig.TeamNameColor;
                targetTeamConfig.DateTimeColor = sourceTeamConfig.DateTimeColor;
                targetTeamConfig.TeamIcon = sourceTeamConfig.TeamIcon;
                targetTeamConfig.UpdatedAt = DateTimeOffset.UtcNow;
            }

            // 2. Clone Webhooks
            var sourceWebhooks = await _context.BingoWebhooks
                .Where(w => w.CharacterName.ToLower() == sourceNameLower)
                .ToListAsync();

            if (sourceWebhooks.Any())
            {
                foreach (var sw in sourceWebhooks)
                {
                    _context.BingoWebhooks.Add(new BingoWebhook
                    {
                        Id = Guid.NewGuid(),
                        CharacterName = cloneDto.TargetCharacterName,
                        WebhookUrl = sw.WebhookUrl,
                        CreatedAt = DateTimeOffset.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cloning bingo config from {Source} to {Target}", 
                cloneDto.SourceCharacterName, cloneDto.TargetCharacterName);
            throw;
        }
    }

    public async Task CloneBingoConfigsBulkAsync(List<BingoConfigCloneDto> cloneDtos)
    {
        try
        {
            var sourceNames = cloneDtos.Select(c => c.SourceCharacterName.ToLower()).Distinct().ToList();
            var targetNames = cloneDtos.Select(c => c.TargetCharacterName.ToLower()).Distinct().ToList();
            var allNames = sourceNames.Concat(targetNames).Distinct().ToList();

            // Fetch team configs for both source and target names
            var teamConfigs = await _context.BingoTeamConfigs
                .Where(tc => allNames.Contains(tc.CharacterName.ToLower()))
                .ToListAsync();

            // Fetch webhooks for source names
            var sourceWebhooks = await _context.BingoWebhooks
                .Where(w => sourceNames.Contains(w.CharacterName.ToLower()))
                .ToListAsync();

            foreach (var cloneDto in cloneDtos)
            {
                var sourceNameLower = cloneDto.SourceCharacterName.ToLower();
                var targetNameLower = cloneDto.TargetCharacterName.ToLower();

                // 1. Clone Team Config
                var sourceTeamConfig = teamConfigs
                    .FirstOrDefault(tc => tc.CharacterName.ToLower() == sourceNameLower);

                if (sourceTeamConfig != null)
                {
                    var targetTeamConfig = teamConfigs
                        .FirstOrDefault(tc => tc.CharacterName.ToLower() == targetNameLower);

                    if (targetTeamConfig == null)
                    {
                        targetTeamConfig = new BingoTeamConfig
                        {
                            Id = Guid.NewGuid(),
                            CharacterName = cloneDto.TargetCharacterName,
                            CreatedAt = DateTimeOffset.UtcNow
                        };
                        _context.BingoTeamConfigs.Add(targetTeamConfig);
                        teamConfigs.Add(targetTeamConfig); // Add to the local list so we can find it again if it's a source for someone else in this batch
                    }

                    targetTeamConfig.TeamName = sourceTeamConfig.TeamName;
                    targetTeamConfig.TeamNameColor = sourceTeamConfig.TeamNameColor;
                    targetTeamConfig.DateTimeColor = sourceTeamConfig.DateTimeColor;
                    targetTeamConfig.TeamIcon = sourceTeamConfig.TeamIcon;
                    targetTeamConfig.UpdatedAt = DateTimeOffset.UtcNow;
                }

                // 2. Clone Webhooks
                var currentSourceWebhooks = sourceWebhooks
                    .Where(w => w.CharacterName.ToLower() == sourceNameLower)
                    .ToList();

                foreach (var sw in currentSourceWebhooks)
                {
                    _context.BingoWebhooks.Add(new BingoWebhook
                    {
                        Id = Guid.NewGuid(),
                        CharacterName = cloneDto.TargetCharacterName,
                        WebhookUrl = sw.WebhookUrl,
                        CreatedAt = DateTimeOffset.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk cloning bingo configs");
            throw;
        }
    }

    public async Task DeleteBingoConfigAsync(string characterName)
    {
        try
        {
            var characterNameLower = characterName.ToLower();

            var teamConfigs = await _context.BingoTeamConfigs
                .Where(tc => tc.CharacterName.ToLower() == characterNameLower)
                .ToListAsync();

            var webhooks = await _context.BingoWebhooks
                .Where(w => w.CharacterName.ToLower() == characterNameLower)
                .ToListAsync();

            if (teamConfigs.Any())
            {
                _context.BingoTeamConfigs.RemoveRange(teamConfigs);
            }

            if (webhooks.Any())
            {
                _context.BingoWebhooks.RemoveRange(webhooks);
            }

            if (teamConfigs.Any() || webhooks.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting bingo config for character {Character}", characterName);
            throw;
        }
    }

    public List<BingoTeamIconDto> GetTeamIcons()
    {
        return new List<BingoTeamIconDto>
        {
            new BingoTeamIconDto
            {
                TeamName = "Shayzien Shower Skippers",
                TeamIcon = "https://www.emoji.family/api/emojis/1f9a8/twemoji/png/32"
            },
            new BingoTeamIconDto()
            {
                TeamName = "Wunescape’s Wiki Willies",
                TeamIcon = "https://www.google.com/s2/favicons?domain=https://oldschool.runescape.wiki/"
            },
            new BingoTeamIconDto()
            {
                TeamName = "Mory Monster Mashers",
                TeamIcon = "https://cdn.discordapp.com/emojis/380191922590842890.png"
            },
            new BingoTeamIconDto()
            {
                TeamName = "Bedabin Baguette Bandits",
                TeamIcon = "https://www.emoji.family/api/emojis/1f956/twemoji/png/32"
            },
            new BingoTeamIconDto()
            {
                TeamName = "Darkmeyers Degenerate Deviants",
                TeamIcon = "https://i.postimg.cc/jSW-Lq5Xj/image.png"
            },
            new BingoTeamIconDto()
            {
                TeamName = "Fantastic Falador Fremboys",
                TeamIcon = "https://i.postimg.cc/Dz0k3Jd1/7c3fda67f4f0a6a7.png"
            },
            new BingoTeamIconDto()
            {
                TeamName = "Gielinor Gooners",
                TeamIcon = "https://cdn.discordapp.com/emojis/1477799405653463150.png"
            }
            
        };
    }

    public async Task<List<BingoTeamMappingDto>> GetAllTeamMappingsAsync()
    {
        try
        {
            return await _context.BingoTeamConfigs
                .Select(tc => new BingoTeamMappingDto
                {
                    // OSRS represents spaces in player names as non-breaking spaces (\u00a0) in the game engine.
                    // This endpoint exists solely to serve the faux-bingo plugin's chat icon lookup, so we
                    // normalise here to match what the plugin receives from chat events.
                    Character = tc.CharacterName.Replace(' ', '\u00a0'),
                    TeamName = tc.TeamName
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all team mappings");
            throw;
        }
    }
}
