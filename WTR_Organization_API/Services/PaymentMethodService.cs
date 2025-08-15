using Microsoft.EntityFrameworkCore;
using WTR_Organization_API.Models;
using WTROrganization.Data;
using WTROrganization.DTOs;
using WTROrganization.Models;

namespace WTROrganization.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly InventoryDbContext _context;

        public PaymentMethodService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PaymentMethodDto>> GetAllPaymentMethodsAsync()
        {
            return await _context.PaymentMethods
                .Where(pm => pm.IsActive)
                .Select(pm => new PaymentMethodDto
                {
                    PaymentMethodId = pm.PaymentMethodId,
                    MethodName = pm.MethodName,
                    MethodCode = pm.MethodCode,
                    IsActive = pm.IsActive
                })
                .ToListAsync();
        }

        public async Task<PaymentMethodDto?> GetPaymentMethodByIdAsync(int id)
        {
            var paymentMethod = await _context.PaymentMethods
                .Where(pm => pm.PaymentMethodId == id && pm.IsActive)
                .FirstOrDefaultAsync();

            if (paymentMethod == null) return null;

            return new PaymentMethodDto
            {
                PaymentMethodId = paymentMethod.PaymentMethodId,
                MethodName = paymentMethod.MethodName,
                MethodCode = paymentMethod.MethodCode,
                IsActive = paymentMethod.IsActive
            };
        }

        public async Task<PaymentMethodDto?> GetPaymentMethodByCodeAsync(string code)
        {
            var paymentMethod = await _context.PaymentMethods
                .Where(pm => pm.MethodCode == code && pm.IsActive)
                .FirstOrDefaultAsync();

            if (paymentMethod == null) return null;

            return new PaymentMethodDto
            {
                PaymentMethodId = paymentMethod.PaymentMethodId,
                MethodName = paymentMethod.MethodName,
                MethodCode = paymentMethod.MethodCode,
                IsActive = paymentMethod.IsActive
            };
        }

        public async Task<PaymentMethodDto> CreatePaymentMethodAsync(CreatePaymentMethodDto createPaymentMethodDto)
        {
            var paymentMethod = new PaymentMethod
            {
                MethodName = createPaymentMethodDto.MethodName,
                MethodCode = createPaymentMethodDto.MethodCode,
                IsActive = true,
                CreateDate = DateTime.Now
            };

            _context.PaymentMethods.Add(paymentMethod);
            await _context.SaveChangesAsync();

            return new PaymentMethodDto
            {
                PaymentMethodId = paymentMethod.PaymentMethodId,
                MethodName = paymentMethod.MethodName,
                MethodCode = paymentMethod.MethodCode,
                IsActive = paymentMethod.IsActive
            };
        }

        public async Task<PaymentMethodDto?> UpdatePaymentMethodAsync(int id, CreatePaymentMethodDto updatePaymentMethodDto)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (paymentMethod == null || !paymentMethod.IsActive) return null;

            paymentMethod.MethodName = updatePaymentMethodDto.MethodName;
            paymentMethod.MethodCode = updatePaymentMethodDto.MethodCode;

            await _context.SaveChangesAsync();

            return new PaymentMethodDto
            {
                PaymentMethodId = paymentMethod.PaymentMethodId,
                MethodName = paymentMethod.MethodName,
                MethodCode = paymentMethod.MethodCode,
                IsActive = paymentMethod.IsActive
            };
        }

        public async Task<bool> DeletePaymentMethodAsync(int id)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (paymentMethod == null) return false;

            paymentMethod.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}