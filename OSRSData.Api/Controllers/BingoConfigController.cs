using System;
using System.Collections.Generic;
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

    [HttpPut]
    public async Task<IActionResult> UpdateConfig(
        [FromQuery] string character,
        [FromBody] BingoTeamConfigUpdateDto updateDto)
    {
        if (string.IsNullOrWhiteSpace(character))
        {
            return BadRequest(new { error = "Character name is required" });
        }

        if (updateDto == null)
        {
            return BadRequest(new { error = "Update data is required" });
        }

        try
        {
            await _bingoService.UpdateBingoTeamConfigAsync(character, updateDto);
            return Ok(new { message = "Bingo team config updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating bingo config for character {Character}", character);
            return StatusCode(500, new { error = "An error occurred processing your request" });
        }
    }

    [HttpPost("teams")]
    public async Task<IActionResult> UpdateConfigsBulk([FromBody] List<BingoTeamConfigBulkDto> configs)
    {
        if (configs == null || configs.Count == 0)
        {
            return BadRequest(new { error = "Config list is required and cannot be empty" });
        }

        try
        {
            await _bingoService.UpdateBingoTeamConfigsBulkAsync(configs);
            return Ok(new { message = "Bingo team configs updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk updating bingo team configs");
            return StatusCode(500, new { error = "An error occurred processing your request" });
        }
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItems([FromBody] List<BingoItemDto> items)
    {
        if (items == null || items.Count == 0)
        {
            return BadRequest(new { error = "Items list is required and cannot be empty" });
        }

        try
        {
            await _bingoService.AddBingoItemsAsync(items);
            return Ok(new { message = "Bingo items added/updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding bingo items");
            return StatusCode(500, new { error = "An error occurred processing your request" });
        }
    }

    [HttpPost("webhooks")]
    public async Task<IActionResult> AddWebhook([FromBody] BingoWebhookUpdateDto webhookDto)
    {
        if (webhookDto == null || string.IsNullOrWhiteSpace(webhookDto.CharacterName) || string.IsNullOrWhiteSpace(webhookDto.WebhookUrl))
        {
            return BadRequest(new { error = "Character name and Webhook URL are required" });
        }

        try
        {
            await _bingoService.AddBingoWebhookAsync(webhookDto);
            return Ok(new { message = "Bingo webhook added successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding bingo webhook");
            return StatusCode(500, new { error = "An error occurred processing your request" });
        }
    }

    [HttpPost("webhooks/bulk")]
    public async Task<IActionResult> AddWebhooksBulk([FromBody] List<BingoWebhookUpdateDto> webhooks)
    {
        if (webhooks == null || webhooks.Count == 0)
        {
            return BadRequest(new { error = "Webhook list is required and cannot be empty" });
        }

        try
        {
            await _bingoService.AddBingoWebhooksBulkAsync(webhooks);
            return Ok(new { message = "Bingo webhooks added successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk adding bingo webhooks");
            return StatusCode(500, new { error = "An error occurred processing your request" });
        }
    }
}
