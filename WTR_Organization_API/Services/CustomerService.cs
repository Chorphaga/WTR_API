using Microsoft.EntityFrameworkCore;
using WTROrganization.Data;
using WTROrganization.DTOs;
using WTROrganization.Models;

namespace WTROrganization.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly InventoryDbContext _context;

        public CustomerService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Where(c => c.IsActive)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.Name,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    CustomerType = c.CustomerType,
                    IsActive = c.IsActive
                })
                .ToListAsync();
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
        {
            var customer = await _context.Customers
                .Where(c => c.CustomerId == id && c.IsActive)
                .FirstOrDefaultAsync();

            if (customer == null) return null;

            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                CustomerType = customer.CustomerType,
                IsActive = customer.IsActive
            };
        }

        public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto)
        {
            var customer = new Customer
            {
                Name = createCustomerDto.Name,
                PhoneNumber = createCustomerDto.PhoneNumber,
                Address = createCustomerDto.Address,
                CustomerType = createCustomerDto.CustomerType,
                IsActive = true,
                CreateDate = DateTime.Now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                CustomerType = customer.CustomerType,
                IsActive = customer.IsActive
            };
        }

        public async Task<CustomerDto?> UpdateCustomerAsync(int id, CreateCustomerDto updateCustomerDto)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null || !customer.IsActive) return null;

            customer.Name = updateCustomerDto.Name;
            customer.PhoneNumber = updateCustomerDto.PhoneNumber;
            customer.Address = updateCustomerDto.Address;
            customer.CustomerType = updateCustomerDto.CustomerType;
            customer.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                CustomerType = customer.CustomerType,
                IsActive = customer.IsActive
            };
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;

            customer.IsActive = false;
            customer.UpdateDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}