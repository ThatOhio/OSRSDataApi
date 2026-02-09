using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OSRSData.App.DTOs;
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

            return new BingoConfigResponseDto
            {
                Webhooks = webhooks,
                Items = items
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching bingo config for character {Character}", characterName);
            throw;
        }
    }
}
