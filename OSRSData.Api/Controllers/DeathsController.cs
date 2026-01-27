using Microsoft.AspNetCore.Mvc;
using OSRSData.App.DTOs;
using OSRSData.App.Services;

namespace OSRSData.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeathsController : ControllerBase
{
    private readonly ILogService _logService;
    private readonly ILogger<DeathsController> _logger;

    public DeathsController(ILogService logService, ILogger<DeathsController> logger)
    {
        _logService = logService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> IngestDeath([FromBody] DeathEntryDto deathEntry)
    {
        if (deathEntry == null)
        {
            return BadRequest("No death log provided.");
        }

        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers.UserAgent.ToString();
            
            await _logService.ProcessDeathRecordAsync(deathEntry, ipAddress, userAgent);
            return Ok(new { message = "Death log ingested successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting death log");
            return StatusCode(500, "An error occurred while processing the death log.");
        }
    }
}
