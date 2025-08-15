using WTROrganization.DTOs;

namespace WTROrganization.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto);
        Task<EmployeeDto?> UpdateEmployeeAsync(int id, CreateEmployeeDto updateEmployeeDto);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}