using Safi.Dto.Prices;
using Safi.Models;

namespace Safi.Interfaces
{
    public interface IPrices
    {
        Task<List<GetPriceDto>> GetAllAsync();
        Task<List<GetPriceDto>> GetAllPricesServiceAsync(string ServiceName);
        Task<GetPriceDto?> GetByIdAsync(int id);
        Task<Prices> CreateAsync(CreatePriceDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<GetPriceDto>> GetCurrentPricesAsync();
        Task<List<GetPriceDto>> GetActivePricesAsync(string ServiceName);
        Task<Prices> GetTheActivePriceNowAsync(string ServiceName);
        Task<decimal> GetPriceNowAsync(string ServiceName);
        Task<decimal> GetPriceByIdAsync(int Id);
        Task<decimal> GetPriceInSpecificDateAsync(string ServiceName,DateOnly date);
    }
}
