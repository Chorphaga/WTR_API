using System.ComponentModel.DataAnnotations;

namespace WTROrganization.DTOs
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? CustomerType { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCustomerDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? CustomerType { get; set; }
    }
}