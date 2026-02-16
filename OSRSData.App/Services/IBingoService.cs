using System.Collections.Generic;
using System.Threading.Tasks;
using OSRSData.App.DTOs;

namespace OSRSData.App.Services;

public interface IBingoService
{
    Task<BingoConfigResponseDto> GetBingoConfigAsync(string characterName);
    Task UpdateBingoTeamConfigAsync(string characterName, BingoTeamConfigUpdateDto updateDto);
    Task AddBingoItemsAsync(List<BingoItemDto> items);
    Task AddBingoWebhookAsync(BingoWebhookUpdateDto webhookDto);
}
