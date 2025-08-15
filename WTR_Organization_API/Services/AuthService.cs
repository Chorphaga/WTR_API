using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WTROrganization.Data;
using WTROrganization.DTOs;
using WTROrganization.Models;
using BCrypt.Net;

namespace WTROrganization.Services
{
    public class AuthService : IAuthService
    {
        private readonly InventoryDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(InventoryDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // เปลี่ยนจากการค้นหาด้วย IdCardNumber เป็น EmployeeId
            var employee = await _context.Employees
                .Where(e => e.EmployeeId == loginDto.EmployeeId && e.IsActive)
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                Console.WriteLine("❌ ไม่พบพนักงาน");
                return null;
            }

            if (string.IsNullOrEmpty(employee.PasswordHash))
            {
                Console.WriteLine("⚠️ ยังไม่ได้ตั้งรหัสผ่าน");
                return null;
            }


            // ตรวจสอบรหัสผ่าน
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, employee.PasswordHash))
                return null;

            var employeeDto = new EmployeeDto
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

            var token = GenerateJwtToken(employeeDto);
            var expiresAt = DateTime.Now.AddHours(
                int.Parse(_configuration["Jwt:ExpiryInHours"] ?? "24"));

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                Employee = employeeDto
            };
        }

        public async Task<bool> ChangePasswordAsync(int employeeId, ChangePasswordDto changePasswordDto)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null || !employee.IsActive)
                return false;

            // ตรวจสอบรหัสผ่านเดิม
            if (string.IsNullOrEmpty(employee.PasswordHash) ||
                !BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, employee.PasswordHash))
                return false;

            // อัพเดทรหัสผ่านใหม่
            employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            employee.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EmployeeDto?> GetCurrentUserAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Where(e => e.EmployeeId == employeeId && e.IsActive)
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

        public string GenerateJwtToken(EmployeeDto employee)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, employee.EmployeeId.ToString()),
                    new Claim(ClaimTypes.Name, $"{employee.FirstName} {employee.LastName}"),
                    new Claim("EmployeeId", employee.EmployeeId.ToString()),
                    new Claim(ClaimTypes.Role, employee.Role ?? "Employee")
                }),
                Expires = DateTime.UtcNow.AddHours(
                    int.Parse(_configuration["Jwt:ExpiryInHours"] ?? "24")),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<SignupResponseDto> SignupAsync(SignupDto signupDto)
        {
            try
            {
                // ตรวจสอบว่า Employee ID มีอยู่จริงและยังไม่ได้ตั้งรหัสผ่าน
                var employee = await _context.Employees
                    .Where(e => e.EmployeeId == signupDto.EmployeeId && e.IsActive)
                    .FirstOrDefaultAsync();

                if (employee == null)
                {
                    return new SignupResponseDto
                    {
                        Success = false,
                        Message = "ไม่พบข้อมูลพนักงานดังกล่าว"
                    };
                }

                if (!string.IsNullOrEmpty(employee.PasswordHash))
                {
                    return new SignupResponseDto
                    {
                        Success = false,
                        Message = "พนักงานท่านนี้ได้ลงทะเบียนแล้ว"
                    };
                }

                // ตั้งรหัสผ่านใหม่
                employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(signupDto.Password);
                employee.UpdateDate = DateTime.Now;

                await _context.SaveChangesAsync();

                var employeeDto = new EmployeeDto
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

                return new SignupResponseDto
                {
                    Success = true,
                    Message = "ลงทะเบียนสำเร็จ",
                    Employee = employeeDto
                };
            }
            catch (Exception ex)
            {
                return new SignupResponseDto
                {
                    Success = false,
                    Message = "เกิดข้อผิดพลาดในการลงทะเบียน: " + ex.Message
                };
            }
        }
    }
}