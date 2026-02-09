using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OSRSData.App.DTOs;
using OSRSData.App.Services;

namespace OSRSData.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BingoConfigController : ControllerBase
{
    private readonly IBingoService _bingoService;
    private readonly ILogger<BingoConfigController> _logger;

    public BingoConfigController(IBingoService bingoService, ILogger<BingoConfigController> logger)
    {
        _bingoService = bingoService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> GetConfig(
        [FromQuery] string character,
        [FromBody] BingoConfigRequestDto? request)
    {
        // Validate character parameter
        if (string.IsNullOrWhiteSpace(character))
        {
            return BadRequest(new { error = "Character name is required" });
        }

        try
        {
            // Call service
            var config = await _bingoService.GetBingoConfigAsync(character);
            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching bingo config for character {Character}", character);
            return StatusCode(500, new { error = "An error occurred processing your request" });
        }
    }
}
