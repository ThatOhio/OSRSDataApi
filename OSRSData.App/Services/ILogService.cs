using OSRSData.App.DTOs;

namespace OSRSData.App.Services;

public interface ILogService
{
    Task ProcessLogsAsync(IEnumerable<LogEntryDto> logs, string? ipAddress, string? userAgent);
    Task ProcessDeathRecordAsync(DeathEntryDto deathEntry, string? ipAddress, string? userAgent);
}
