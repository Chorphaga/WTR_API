using WTROrganization.DTOs;

namespace WTROrganization.Services
{
    public interface ICompanyService
    {
        Task<CompanySettingsDto?> GetCompanySettingsAsync();
        Task<CompanySettingsDto> CreateOrUpdateCompanySettingsAsync(CreateCompanySettingsDto dto);
        Task<bool> DeleteCompanySettingsAsync(int id);
    }
}