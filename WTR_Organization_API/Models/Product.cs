using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTROrganization.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [StringLength(150)]
        public string? ProductName { get; set; }

        [StringLength(50)]
        public string? Unit { get; set; }

        public int Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal NormalPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PartnerPrice { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public int? CreateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int? UpdateBy { get; set; }

        // Navigation properties
        public virtual ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();
    }
}