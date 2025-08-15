using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTROrganization.Models
{
    public class Bill
    {
        [Key]
        public int BillId { get; set; }

        [StringLength(50)]
        public string? BillType { get; set; }

        public int EmployeeId { get; set; }

        public int CustomerId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; } // Keep for backward compatibility

        [StringLength(50)]
        public string? BillStatus { get; set; }

        [StringLength(255)]
        public string? Remark { get; set; }

        // ฟิลด์ใหม่สำหรับการชำระเงิน
        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [StringLength(50)]
        public string? PaymentStatus { get; set; }

        public DateTime? DueDate { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal VatRate { get; set; } = 0.00m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal VatAmount { get; set; } = 0.00m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GrandTotal { get; set; }

        [StringLength(50)]
        public string? InvoiceNumber { get; set; }

        // ข้อมูลผู้เกี่ยวข้อง
        public int? ApprovedBy { get; set; }

        [StringLength(100)]
        public string? ReceivedBy { get; set; }

        [StringLength(100)]
        public string? CheckedBy { get; set; }

        [StringLength(100)]
        public string? DeliveryBy { get; set; }

        [StringLength(200)]
        public string? PaymentTerms { get; set; }

        // ฟิลด์ระบบเดิม
        public bool IsActive { get; set; } = true;

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public int? CreateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int? UpdateBy { get; set; }

        // Foreign Key properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual Customer Customer { get; set; } = null!;
        public virtual Employee? ApprovedByEmployee { get; set; }

        // Navigation properties
        public virtual ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();
    }
}