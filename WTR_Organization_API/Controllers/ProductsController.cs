using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WTROrganization.DTOs;
using WTROrganization.Services;

namespace WTROrganization.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// ดึงรายการสินค้าทั้งหมด
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการดึงข้อมูลสินค้า",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ดึงสินค้าตาม ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                    return NotFound(new { message = "ไม่พบสินค้าที่ระบุ" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการดึงข้อมูลสินค้า",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ดึงสินค้าที่มีสต็อกเหลือน้อย
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStockProducts()
        {
            try
            {
                var lowStockProducts = await _productService.GetLowStockProductsAsync();
                return Ok(lowStockProducts);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการดึงข้อมูลสินค้าที่เหลือน้อย",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// เพิ่มสินค้าใหม่
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
        {
            try
            {
                var product = await _productService.CreateProductAsync(createProductDto);
                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการเพิ่มสินค้า",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// อัพเดทข้อมูลสินค้า
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, CreateProductDto updateProductDto)
        {
            try
            {
                var product = await _productService.UpdateProductAsync(id, updateProductDto);

                if (product == null)
                    return NotFound(new { message = "ไม่พบสินค้าที่ต้องการแก้ไข" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการแก้ไขสินค้า",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// อัพเดทจำนวนสต็อกสินค้า
        /// </summary>
        [HttpPatch("{id}/quantity")]
        public async Task<ActionResult<ProductDto>> UpdateProductQuantity(int id, [FromBody] int newQuantity)
        {
            try
            {
                if (newQuantity < 0)
                    return BadRequest(new { message = "จำนวนสินค้าต้องไม่น้อยกว่า 0" });

                var product = await _productService.UpdateProductQuantityAsync(id, newQuantity);

                if (product == null)
                    return NotFound(new { message = "ไม่พบสินค้าที่ต้องการแก้ไข" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการแก้ไขจำนวนสินค้า",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// อัพเดทราคาสินค้า
        /// </summary>
        [HttpPatch("{id}/prices")]
        public async Task<ActionResult<ProductDto>> UpdateProductPrices(int id, [FromBody] UpdateProductPricesDto pricesDto)
        {
            try
            {
                if (pricesDto.NormalPrice < 0 || pricesDto.PartnerPrice < 0)
                    return BadRequest(new { message = "ราคาต้องไม่น้อยกว่า 0" });

                var product = await _productService.UpdateProductPricesAsync(id, pricesDto.NormalPrice, pricesDto.PartnerPrice);

                if (product == null)
                    return NotFound(new { message = "ไม่พบสินค้าที่ต้องการแก้ไข" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการแก้ไขราคาสินค้า",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ลบสินค้า (Soft Delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);

                if (!result)
                    return NotFound(new { message = "ไม่พบสินค้าที่ต้องการลบ" });

                return Ok(new { message = "ลบสินค้าสำเร็จ" });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการลบสินค้า",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ค้นหาสินค้าตามชื่อ
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return BadRequest(new { message = "กรุณาระบุคำค้นหา" });

                var products = await _productService.GetAllProductsAsync();
                var filteredProducts = products.Where(p =>
                    p.ProductName != null &&
                    p.ProductName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                return Ok(filteredProducts);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการค้นหาสินค้า",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ดึงสถิติสินค้า
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult> GetProductStatistics()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                var lowStockProducts = await _productService.GetLowStockProductsAsync();

                var stats = new
                {
                    TotalProducts = products.Count(),
                    ActiveProducts = products.Count(p => p.IsActive),
                    LowStockProducts = lowStockProducts.Count(),
                    OutOfStockProducts = products.Count(p => p.Amount <= 0),
                    TotalValue = products.Sum(p => p.Amount * p.NormalPrice),
                    AveragePrice = products.Any() ? products.Average(p => p.NormalPrice) : 0
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการดึงสถิติสินค้า",
                    error = ex.Message
                });
            }
        }
    }
}