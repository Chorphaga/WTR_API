using WTROrganization.DTOs;

namespace WTROrganization.Services
{
    public interface IStockService
    {
        Task<IEnumerable<StockDto>> GetAllStocksAsync();
        Task<StockDto?> GetStockByIdAsync(int id);
        Task<IEnumerable<StockDto>> GetLowStockItemsAsync();
        Task<StockDto> CreateStockAsync(CreateStockDto createStockDto);
        Task<StockDto?> UpdateStockAsync(int id, CreateStockDto updateStockDto);
        Task<StockDto?> UpdateStockQuantityAsync(int id, int newQuantity);
        Task<bool> DeleteStockAsync(int id);
    }
}