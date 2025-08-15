using System.ComponentModel.DataAnnotations;

namespace WTROrganization.DTOs
{
    public class PaymentMethodDto
    {
        public int PaymentMethodId { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public string MethodCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreatePaymentMethodDto
    {
        [Required]
        public string MethodName { get; set; } = string.Empty;

        [Required]
        public string MethodCode { get; set; } = string.Empty;
    }
}
