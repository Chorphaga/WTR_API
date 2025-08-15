using System.ComponentModel.DataAnnotations;

namespace WTROrganization.DTOs
{
    public class SignupDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "รหัสผ่านไม่ตรงกัน")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class SignupResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public EmployeeDto? Employee { get; set; }
    }

    public class LoginDto
    {
        [Required]
        public int EmployeeId { get; set; } // เปลี่ยนจาก IdCardNumber เป็น EmployeeId

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public EmployeeDto Employee { get; set; } = new EmployeeDto();
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;
    }
}