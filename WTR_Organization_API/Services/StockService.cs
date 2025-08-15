using Microsoft.EntityFrameworkCore;
using WTROrganization.Data;
using WTROrganization.DTOs;
using WTROrganization.Models;

namespace WTROrganization.Services
{
    public class StockService : IStockService
    {
        private readonly InventoryDbContext _context;

        public StockService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockDto>> GetAllStocksAsync()
        {
            return await _context.Stocks
                .Where(s => s.IsActive)
                .Select(s => new StockDto
                {
                    ItemId = s.ItemId,
                    ItemName = s.ItemName,
                    Unit = s.Unit,
                    Amount = s.Amount,
                    ImportPrice = s.ImportPrice,
                    ExportPrice = s.ExportPrice,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<StockDto?> GetStockByIdAsync(int id)
        {
            var stock = await _context.Stocks
                .Where(s => s.ItemId == id && s.IsActive)
                .FirstOrDefaultAsync();

            if (stock == null) return null;

            return new StockDto
            {
                ItemId = stock.ItemId,
                ItemName = stock.ItemName,
                Unit = stock.Unit,
                Amount = stock.Amount,
                ImportPrice = stock.ImportPrice,
                ExportPrice = stock.ExportPrice,
                IsActive = stock.IsActive
            };
        }

        public async Task<IEnumerable<StockDto>> GetLowStockItemsAsync()
        {
            return await _context.Stocks
                .Where(s => s.IsActive && s.Amount <= 10)
                .Select(s => new StockDto
                {
                    ItemId = s.ItemId,
                    ItemName = s.ItemName,
                    Unit = s.Unit,
                    Amount = s.Amount,
                    ImportPrice = s.ImportPrice,
                    ExportPrice = s.ExportPrice,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<StockDto> CreateStockAsync(CreateStockDto createStockDto)
        {
            var stock = new Stock
            {
                ItemName = createStockDto.ItemName,
                Unit = createStockDto.Unit,
                Amount = createStockDto.Amount,
                ImportPrice = createStockDto.ImportPrice,
                ExportPrice = createStockDto.ExportPrice,
                IsActive = true,
                CreateDate = DateTime.Now
            };

            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();

            return new StockDto
            {
                ItemId = stock.ItemId,
                ItemName = stock.ItemName,
                Unit = stock.Unit,
                Amount = stock.Amount,
                ImportPrice = stock.ImportPrice,
                ExportPrice = stock.ExportPrice,
                IsActive = stock.IsActive
            };
        }

        public async Task<StockDto?> UpdateStockAsync(int id, CreateStockDto updateStockDto)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null || !stock.IsActive) return null;

            stock.ItemName = updateStockDto.ItemName;
            stock.Unit = updateStockDto.Unit;
            stock.Amount = updateStockDto.Amount;
            stock.ImportPrice = updateStockDto.ImportPrice;
            stock.ExportPrice = updateStockDto.ExportPrice;
            stock.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return new StockDto
            {
                ItemId = stock.ItemId,
                ItemName = stock.ItemName,
                Unit = stock.Unit,
                Amount = stock.Amount,
                ImportPrice = stock.ImportPrice,
                ExportPrice = stock.ExportPrice,
                IsActive = stock.IsActive
            };
        }

        public async Task<StockDto?> UpdateStockQuantityAsync(int id, int newQuantity)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null || !stock.IsActive) return null;

            stock.Amount = newQuantity;
            stock.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return new StockDto
            {
                ItemId = stock.ItemId,
                ItemName = stock.ItemName,
                Unit = stock.Unit,
                Amount = stock.Amount,
                ImportPrice = stock.ImportPrice,
                ExportPrice = stock.ExportPrice,
                IsActive = stock.IsActive
            };
        }

        public async Task<bool> DeleteStockAsync(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null) return false;

            stock.IsActive = false;
            stock.UpdateDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}