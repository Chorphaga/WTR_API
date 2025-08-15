using System.ComponentModel.DataAnnotations;

namespace WTROrganization.DTOs
{
    public class BillDto
    {
        public int BillId { get; set; }
        public string? BillType { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public decimal TotalPrice { get; set; } // Keep for backward compatibility
        public string? BillStatus { get; set; }
        public string? Remark { get; set; }
        public DateTime CreateDate { get; set; }

        // ฟิลด์ใหม่
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal VatRate { get; set; }
        public decimal VatAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string? InvoiceNumber { get; set; }
        public int? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public string? ReceivedBy { get; set; }
        public string? CheckedBy { get; set; }
        public string? DeliveryBy { get; set; }
        public string? PaymentTerms { get; set; }

        public List<BillItemDto> BillItems { get; set; } = new List<BillItemDto>();
    }

    public class CreateBillDto
    {
        [Required]
        public string BillType { get; set; } = string.Empty;

        public int EmployeeId { get; set; }
        public int CustomerId { get; set; }
        public string? BillStatus { get; set; } = "รอชำระ";
        public string? Remark { get; set; }

        // ฟิลด์ใหม่
        public string? PaymentMethod { get; set; } = "CASH";
        public string? PaymentStatus { get; set; } = "รอชำระ";
        public DateTime? DueDate { get; set; }
        public decimal? VatRate { get; set; } = 0.00m;
        public string? InvoiceNumber { get; set; }
        public int? ApprovedBy { get; set; }
        public string? ReceivedBy { get; set; }
        public string? CheckedBy { get; set; }
        public string? DeliveryBy { get; set; }
        public string? PaymentTerms { get; set; }

        public List<CreateBillItemDto> BillItems { get; set; } = new List<CreateBillItemDto>();
    }

    // DTOs สำหรับอัพเดทแยกส่วน
    public class UpdateBillPaymentDto
    {
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? DueDate { get; set; }
        public string? PaymentTerms { get; set; }
    }

    public class UpdateBillVatDto
    {
        [Range(0, 100)]
        public decimal VatRate { get; set; }
    }

    public class UpdateBillPeopleDto
    {
        public int? ApprovedBy { get; set; }
        public string? ReceivedBy { get; set; }
        public string? CheckedBy { get; set; }
        public string? DeliveryBy { get; set; }
    }
}