using System.Collections.Generic;
using System.Threading.Tasks;
using OSRSData.App.DTOs;

namespace OSRSData.App.Services;

public interface IBingoService
{
    Task<BingoConfigResponseDto> GetBingoConfigAsync(string characterName);
    Task UpdateBingoTeamConfigAsync(string characterName, BingoTeamConfigUpdateDto updateDto);
    Task UpdateBingoTeamConfigsBulkAsync(List<BingoTeamConfigBulkDto> configs);
    Task AddBingoItemsAsync(List<BingoItemDto> items);
    Task AddBingoWebhookAsync(BingoWebhookUpdateDto webhookDto);
    Task AddBingoWebhooksBulkAsync(List<BingoWebhookUpdateDto> webhooks);
    Task CloneBingoConfigAsync(BingoConfigCloneDto cloneDto);
    Task CloneBingoConfigsBulkAsync(List<BingoConfigCloneDto> cloneDtos);
    Task DeleteBingoConfigAsync(string characterName);
    List<BingoTeamIconDto> GetTeamIcons();
    Task<List<BingoTeamMappingDto>> GetAllTeamMappingsAsync();
}
