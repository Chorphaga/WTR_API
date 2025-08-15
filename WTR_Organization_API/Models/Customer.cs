using System.ComponentModel.DataAnnotations;

namespace WTROrganization.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [StringLength(150)]
        public string? Name { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? CustomerType { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public int? CreateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int? UpdateBy { get; set; }

        // Navigation properties
        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
    }
}