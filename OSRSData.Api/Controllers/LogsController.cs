using Microsoft.AspNetCore.Mvc;
using OSRSData.App.DTOs;
using OSRSData.App.Services;

namespace OSRSData.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly ILogService _logService;
    private readonly ILogger<LogsController> _logger;

    public LogsController(ILogService logService, ILogger<LogsController> logger)
    {
        _logService = logService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> IngestLogs([FromBody] IEnumerable<LogEntryDto> logs)
    {
        if (logs == null || !logs.Any())
        {
            return BadRequest("No logs provided.");
        }

        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers.UserAgent.ToString();
            
            await _logService.ProcessLogsAsync(logs, ipAddress, userAgent);
            return Ok(new { message = "Logs ingested successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting logs");
            return StatusCode(500, "An error occurred while processing logs.");
        }
    }
}
