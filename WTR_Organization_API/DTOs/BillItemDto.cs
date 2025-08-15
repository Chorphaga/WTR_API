using System.ComponentModel.DataAnnotations;

namespace WTROrganization.DTOs
{
    public class BillItemDto
    {
        public int BillItemId { get; set; }
        public int BillId { get; set; }
        public int? ItemId { get; set; }
        public string? ItemName { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class CreateBillItemDto
    {
        public int? ItemId { get; set; }
        public int? ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "จำนวนต้องมากกว่า 0")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "ราคาต้องมากกว่า 0")]
        public decimal PricePerUnit { get; set; }
    }
}