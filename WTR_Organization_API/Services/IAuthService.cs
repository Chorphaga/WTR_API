using WTROrganization.DTOs;

namespace WTROrganization.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
        Task<SignupResponseDto> SignupAsync(SignupDto signupDto);
        Task<bool> ChangePasswordAsync(int employeeId, ChangePasswordDto changePasswordDto);
        Task<EmployeeDto?> GetCurrentUserAsync(int employeeId);
        string GenerateJwtToken(EmployeeDto employee);
    }
}