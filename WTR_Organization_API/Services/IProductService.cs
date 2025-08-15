using WTROrganization.DTOs;

namespace WTROrganization.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetLowStockProductsAsync();
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto?> UpdateProductAsync(int id, CreateProductDto updateProductDto);
        Task<ProductDto?> UpdateProductQuantityAsync(int id, int newQuantity);
        Task<ProductDto?> UpdateProductPricesAsync(int id, decimal normalPrice, decimal partnerPrice);
        Task<bool> DeleteProductAsync(int id);
    }
}