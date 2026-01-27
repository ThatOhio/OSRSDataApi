using Microsoft.Extensions.Logging;
using OSRSData.App.DTOs;
using OSRSData.Core.Entities;
using OSRSData.Core.Enums;
using OSRSData.DAL;

namespace OSRSData.App.Services;

public class LogService : ILogService
{
    private readonly OSRSDbContext _context;
    private readonly ILogger<LogService> _logger;

    public LogService(OSRSDbContext context, ILogger<LogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ProcessLogsAsync(IEnumerable<LogEntryDto> logs, string? ipAddress, string? userAgent)
    {
        foreach (var dto in logs)
        {
            if (!Enum.TryParse<LogType>(dto.Type, out var logType))
            {
                _logger.LogWarning("Unknown log type: {Type}", dto.Type);
                continue;
            }

            var entry = new LogEntry
            {
                Id = Guid.NewGuid(),
                Player = dto.Player,
                Type = logType,
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(dto.Timestamp),
                ReceivedAt = DateTimeOffset.UtcNow,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                LootRecord = new LootRecord
                {
                    Id = Guid.NewGuid(),
                    Source = dto.Data.Source,
                    TotalValue = dto.Data.TotalValue,
                    Kc = dto.Data.Kc,
                    Items = dto.Data.Items.Select(i => new LootItem
                    {
                        Id = Guid.NewGuid(),
                        ItemId = i.Id,
                        Name = i.Name,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                }
            };

            _context.LogEntries.Add(entry);
        }

        await _context.SaveChangesAsync();
    }

    public async Task ProcessDeathRecordAsync(DeathEntryDto deathEntry, string? ipAddress, string? userAgent)
    {
        if (!Enum.TryParse<LogType>(deathEntry.Type, out var logType))
        {
            _logger.LogWarning("Unknown log type for death: {Type}", deathEntry.Type);
            logType = LogType.DEATH; // Default to DEATH if it's coming to this endpoint?
        }

        var entry = new LogEntry
        {
            Id = Guid.NewGuid(),
            Player = deathEntry.Player,
            Type = logType,
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(deathEntry.Timestamp),
            ReceivedAt = DateTimeOffset.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            DeathRecord = new DeathRecord
            {
                Id = Guid.NewGuid(),
                RegionId = deathEntry.Data.RegionId,
                Killer = deathEntry.Data.Killer
            }
        };

        _context.LogEntries.Add(entry);
        await _context.SaveChangesAsync();
    }
}
