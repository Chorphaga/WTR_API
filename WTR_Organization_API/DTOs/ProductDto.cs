using System.ComponentModel.DataAnnotations;

namespace WTROrganization.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Unit { get; set; }
        public int Amount { get; set; }
        public decimal NormalPrice { get; set; }
        public decimal PartnerPrice { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateProductDto
    {
        [Required]
        public string ProductName { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public int Amount { get; set; }
        public decimal NormalPrice { get; set; }
        public decimal PartnerPrice { get; set; }
    }

    public class UpdateProductPricesDto
    {
        public decimal NormalPrice { get; set; }
        public decimal PartnerPrice { get; set; }
    }
}