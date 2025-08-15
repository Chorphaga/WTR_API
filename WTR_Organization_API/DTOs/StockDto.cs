using System.ComponentModel.DataAnnotations;

namespace WTROrganization.DTOs
{
    public class StockDto
    {
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? Unit { get; set; }
        public int Amount { get; set; }
        public decimal ImportPrice { get; set; }
        public decimal ExportPrice { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateStockDto
    {
        [Required]
        public string ItemName { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public int Amount { get; set; }
        public decimal ImportPrice { get; set; }
        public decimal ExportPrice { get; set; }
    }
}