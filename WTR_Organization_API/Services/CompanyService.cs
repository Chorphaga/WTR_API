using Microsoft.EntityFrameworkCore;
using WTR_Organization_API.Models;
using WTROrganization.Data;
using WTROrganization.DTOs;
using WTROrganization.Models;

namespace WTROrganization.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly InventoryDbContext _context;

        public CompanyService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<CompanySettingsDto?> GetCompanySettingsAsync()
        {
            var company = await _context.CompanySettings
                .Where(c => c.IsActive)
                .FirstOrDefaultAsync();

            if (company == null) return null;

            return new CompanySettingsDto
            {
                SettingId = company.SettingId,
                CompanyName = company.CompanyName,
                Address = company.Address,
                PhoneNumber = company.PhoneNumber,
                MobileNumber = company.MobileNumber,
                Email = company.Email,
                TaxId = company.TaxId,
                BankAccount = company.BankAccount,
                BankName = company.BankName,
                IsActive = company.IsActive
            };
        }

        public async Task<CompanySettingsDto> CreateOrUpdateCompanySettingsAsync(CreateCompanySettingsDto dto)
        {
            var existingCompany = await _context.CompanySettings
                .Where(c => c.IsActive)
                .FirstOrDefaultAsync();

            if (existingCompany != null)
            {
                // Update existing
                existingCompany.CompanyName = dto.CompanyName;
                existingCompany.Address = dto.Address;
                existingCompany.PhoneNumber = dto.PhoneNumber;
                existingCompany.MobileNumber = dto.MobileNumber;
                existingCompany.Email = dto.Email;
                existingCompany.TaxId = dto.TaxId;
                existingCompany.BankAccount = dto.BankAccount;
                existingCompany.BankName = dto.BankName;
                existingCompany.UpdateDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return new CompanySettingsDto
                {
                    SettingId = existingCompany.SettingId,
                    CompanyName = existingCompany.CompanyName,
                    Address = existingCompany.Address,
                    PhoneNumber = existingCompany.PhoneNumber,
                    MobileNumber = existingCompany.MobileNumber,
                    Email = existingCompany.Email,
                    TaxId = existingCompany.TaxId,
                    BankAccount = existingCompany.BankAccount,
                    BankName = existingCompany.BankName,
                    IsActive = existingCompany.IsActive
                };
            }
            else
            {
                // Create new
                var company = new CompanySettings
                {
                    CompanyName = dto.CompanyName,
                    Address = dto.Address,
                    PhoneNumber = dto.PhoneNumber,
                    MobileNumber = dto.MobileNumber,
                    Email = dto.Email,
                    TaxId = dto.TaxId,
                    BankAccount = dto.BankAccount,
                    BankName = dto.BankName,
                    IsActive = true,
                    CreateDate = DateTime.Now
                };

                _context.CompanySettings.Add(company);
                await _context.SaveChangesAsync();

                return new CompanySettingsDto
                {
                    SettingId = company.SettingId,
                    CompanyName = company.CompanyName,
                    Address = company.Address,
                    PhoneNumber = company.PhoneNumber,
                    MobileNumber = company.MobileNumber,
                    Email = company.Email,
                    TaxId = company.TaxId,
                    BankAccount = company.BankAccount,
                    BankName = company.BankName,
                    IsActive = company.IsActive
                };
            }
        }

        public async Task<bool> DeleteCompanySettingsAsync(int id)
        {
            var company = await _context.CompanySettings.FindAsync(id);
            if (company == null) return false;

            company.IsActive = false;
            company.UpdateDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}