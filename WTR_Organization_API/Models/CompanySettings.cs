using System.ComponentModel.DataAnnotations;

namespace WTR_Organization_API.Models
{
    public class CompanySettings
    {
        [Key]
        public int SettingId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobileNumber { get; set; }
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? TaxId { get; set; }

        [StringLength(200)]
        public string? BankAccount { get; set; }

        [StringLength(100)]
        public string? BankName { get; set; }

        public byte[]? Logo { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public DateTime? UpdateDate { get; set; }

        public int? UpdateBy { get; set; }

    }
}
