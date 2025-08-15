using WTROrganization.DTOs;

namespace WTROrganization.Services
{
    public interface IBillService
    {
        // Methods เดิม
        Task<IEnumerable<BillDto>> GetAllBillsAsync();
        Task<BillDto?> GetBillByIdAsync(int id);
        Task<BillDto> CreateBillAsync(CreateBillDto createBillDto);
        Task<BillDto?> UpdateBillStatusAsync(int id, string status);
        Task<bool> DeleteBillAsync(int id);

        // Methods ใหม่สำหรับ Payment และ VAT
        Task<BillDto?> UpdateBillPaymentAsync(int id, UpdateBillPaymentDto paymentDto);
        Task<BillDto?> UpdateBillVatAsync(int id, UpdateBillVatDto vatDto);
        Task<BillDto?> UpdateBillPeopleAsync(int id, UpdateBillPeopleDto peopleDto);
        Task<BillDto?> RecalculateBillTotalsAsync(int billId);
    }
}