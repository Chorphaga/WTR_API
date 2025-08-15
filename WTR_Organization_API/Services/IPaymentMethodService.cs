using WTROrganization.DTOs;

namespace WTROrganization.Services
{
    public interface IPaymentMethodService
    {
        Task<IEnumerable<PaymentMethodDto>> GetAllPaymentMethodsAsync();
        Task<PaymentMethodDto?> GetPaymentMethodByIdAsync(int id);
        Task<PaymentMethodDto?> GetPaymentMethodByCodeAsync(string code);
        Task<PaymentMethodDto> CreatePaymentMethodAsync(CreatePaymentMethodDto createPaymentMethodDto);
        Task<PaymentMethodDto?> UpdatePaymentMethodAsync(int id, CreatePaymentMethodDto updatePaymentMethodDto);
        Task<bool> DeletePaymentMethodAsync(int id);
    }
}