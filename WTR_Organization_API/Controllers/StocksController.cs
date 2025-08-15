using Microsoft.AspNetCore.Mvc;
using WTROrganization.DTOs;
using WTROrganization.Services;

namespace WTROrganization.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StocksController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetStocks()
        {
            var stocks = await _stockService.GetAllStocksAsync();
            return Ok(stocks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StockDto>> GetStock(int id)
        {
            var stock = await _stockService.GetStockByIdAsync(id);
            if (stock == null)
                return NotFound();

            return Ok(stock);
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetLowStockItems()
        {
            var lowStockItems = await _stockService.GetLowStockItemsAsync();
            return Ok(lowStockItems);
        }

        [HttpPost]
        public async Task<ActionResult<StockDto>> CreateStock(CreateStockDto createStockDto)
        {
            try
            {
                var stock = await _stockService.CreateStockAsync(createStockDto);
                return CreatedAtAction(nameof(GetStock), new { id = stock.ItemId }, stock);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StockDto>> UpdateStock(int id, CreateStockDto updateStockDto)
        {
            var stock = await _stockService.UpdateStockAsync(id, updateStockDto);
            if (stock == null)
                return NotFound();

            return Ok(stock);
        }

        [HttpPatch("{id}/quantity")]
        public async Task<ActionResult<StockDto>> UpdateStockQuantity(int id, [FromBody] int newQuantity)
        {
            var stock = await _stockService.UpdateStockQuantityAsync(id, newQuantity);
            if (stock == null)
                return NotFound();

            return Ok(stock);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var result = await _stockService.DeleteStockAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}