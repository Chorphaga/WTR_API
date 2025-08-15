using System.ComponentModel.DataAnnotations;

namespace WTROrganization.DTOs
{
    public class CompanySettingsDto
    {
        public int SettingId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public string? TaxId { get; set; }
        public string? BankAccount { get; set; }
        public string? BankName { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCompanySettingsDto
    {
        [Required]
        public string CompanyName { get; set; } = string.Empty;

        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public string? TaxId { get; set; }
        public string? BankAccount { get; set; }
        public string? BankName { get; set; }
    }
}
