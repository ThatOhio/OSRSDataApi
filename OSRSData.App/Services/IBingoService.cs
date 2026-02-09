using System.Threading.Tasks;
using OSRSData.App.DTOs;

namespace OSRSData.App.Services;

public interface IBingoService
{
    Task<BingoConfigResponseDto> GetBingoConfigAsync(string characterName);
}
