using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTROrganization.Models
{
    public class BillItem
    {
        [Key]
        public int BillItemId { get; set; }

        public int BillId { get; set; }

        public int? ItemId { get; set; }

        public int? ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerUnit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public int? CreateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int? UpdateBy { get; set; }

        // Foreign Key properties
        public virtual Bill Bill { get; set; } = null!;
        public virtual Stock? Stock { get; set; }
        public virtual Product? Product { get; set; }
    }
}