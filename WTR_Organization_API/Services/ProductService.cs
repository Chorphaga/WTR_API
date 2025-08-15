using Microsoft.EntityFrameworkCore;
using WTROrganization.Data;
using WTROrganization.DTOs;
using WTROrganization.Models;

namespace WTROrganization.Services
{
    public class ProductService : IProductService
    {
        private readonly InventoryDbContext _context;

        public ProductService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Unit = p.Unit,
                    Amount = p.Amount,
                    NormalPrice = p.NormalPrice,
                    PartnerPrice = p.PartnerPrice,
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Where(p => p.ProductId == id && p.IsActive)
                .FirstOrDefaultAsync();

            if (product == null) return null;

            return new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Unit = product.Unit,
                Amount = product.Amount,
                NormalPrice = product.NormalPrice,
                PartnerPrice = product.PartnerPrice,
                IsActive = product.IsActive
            };
        }

        public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive && p.Amount <= 10)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Unit = p.Unit,
                    Amount = p.Amount,
                    NormalPrice = p.NormalPrice,
                    PartnerPrice = p.PartnerPrice,
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = new Product
            {
                ProductName = createProductDto.ProductName,
                Unit = createProductDto.Unit,
                Amount = createProductDto.Amount,
                NormalPrice = createProductDto.NormalPrice,
                PartnerPrice = createProductDto.PartnerPrice,
                IsActive = true,
                CreateDate = DateTime.Now
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Unit = product.Unit,
                Amount = product.Amount,
                NormalPrice = product.NormalPrice,
                PartnerPrice = product.PartnerPrice,
                IsActive = product.IsActive
            };
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, CreateProductDto updateProductDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || !product.IsActive) return null;

            product.ProductName = updateProductDto.ProductName;
            product.Unit = updateProductDto.Unit;
            product.Amount = updateProductDto.Amount;
            product.NormalPrice = updateProductDto.NormalPrice;
            product.PartnerPrice = updateProductDto.PartnerPrice;
            product.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Unit = product.Unit,
                Amount = product.Amount,
                NormalPrice = product.NormalPrice,
                PartnerPrice = product.PartnerPrice,
                IsActive = product.IsActive
            };
        }

        public async Task<ProductDto?> UpdateProductQuantityAsync(int id, int newQuantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || !product.IsActive) return null;

            product.Amount = newQuantity;
            product.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Unit = product.Unit,
                Amount = product.Amount,
                NormalPrice = product.NormalPrice,
                PartnerPrice = product.PartnerPrice,
                IsActive = product.IsActive
            };
        }

        public async Task<ProductDto?> UpdateProductPricesAsync(int id, decimal normalPrice, decimal partnerPrice)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || !product.IsActive) return null;

            product.NormalPrice = normalPrice;
            product.PartnerPrice = partnerPrice;
            product.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Unit = product.Unit,
                Amount = product.Amount,
                NormalPrice = product.NormalPrice,
                PartnerPrice = product.PartnerPrice,
                IsActive = product.IsActive
            };
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.IsActive = false;
            product.UpdateDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}