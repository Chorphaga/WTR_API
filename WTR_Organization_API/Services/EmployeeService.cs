using Microsoft.EntityFrameworkCore;
using WTROrganization.Data;
using WTROrganization.DTOs;
using WTROrganization.Models;
using BCrypt.Net;

namespace WTROrganization.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly InventoryDbContext _context;

        public EmployeeService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Where(e => e.IsActive)
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    IdCardNumber = e.IdCardNumber,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    PhoneNumber = e.PhoneNumber,
                    Address = e.Address,
                    Role = e.Role,
                    IsActive = e.IsActive
                })
                .ToListAsync();
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _context.Employees
                .Where(e => e.EmployeeId == id && e.IsActive)
                .FirstOrDefaultAsync();

            if (employee == null) return null;

            return new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                IdCardNumber = employee.IdCardNumber,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                StartDate = employee.StartDate,
                EndDate = employee.EndDate,
                PhoneNumber = employee.PhoneNumber,
                Address = employee.Address,
                Role = employee.Role,
                IsActive = employee.IsActive
            };
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto)
        {
            var employee = new Employee
            {
                IdCardNumber = createEmployeeDto.IdCardNumber,
                FirstName = createEmployeeDto.FirstName,
                LastName = createEmployeeDto.LastName,
                StartDate = createEmployeeDto.StartDate,
                EndDate = createEmployeeDto.EndDate,
                PhoneNumber = createEmployeeDto.PhoneNumber,
                Address = createEmployeeDto.Address,
                PasswordHash = !string.IsNullOrEmpty(createEmployeeDto.Password)
                    ? BCrypt.Net.BCrypt.HashPassword(createEmployeeDto.Password)
                    : null,
                Role = createEmployeeDto.Role,
                IsActive = true,
                CreateDate = DateTime.Now
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                IdCardNumber = employee.IdCardNumber,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                StartDate = employee.StartDate,
                EndDate = employee.EndDate,
                PhoneNumber = employee.PhoneNumber,
                Address = employee.Address,
                Role = employee.Role,
                IsActive = employee.IsActive
            };
        }

        public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, CreateEmployeeDto updateEmployeeDto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null || !employee.IsActive) return null;

            employee.IdCardNumber = updateEmployeeDto.IdCardNumber;
            employee.FirstName = updateEmployeeDto.FirstName;
            employee.LastName = updateEmployeeDto.LastName;
            employee.StartDate = updateEmployeeDto.StartDate;
            employee.EndDate = updateEmployeeDto.EndDate;
            employee.PhoneNumber = updateEmployeeDto.PhoneNumber;
            employee.Address = updateEmployeeDto.Address;
            employee.Role = updateEmployeeDto.Role;
            employee.UpdateDate = DateTime.Now;

            if (!string.IsNullOrEmpty(updateEmployeeDto.Password))
            {
                employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateEmployeeDto.Password);
            }

            await _context.SaveChangesAsync();

            return new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                IdCardNumber = employee.IdCardNumber,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                StartDate = employee.StartDate,
                EndDate = employee.EndDate,
                PhoneNumber = employee.PhoneNumber,
                Address = employee.Address,
                Role = employee.Role,
                IsActive = employee.IsActive
            };
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;

            employee.IsActive = false;
            employee.UpdateDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}