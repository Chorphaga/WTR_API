using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WTROrganization.DTOs;
using WTROrganization.Services;

namespace WTROrganization.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                if (result == null)
                    return Unauthorized(new { message = "รหัสบัตรประชาชนหรือรหัสผ่านไม่ถูกต้อง" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "เกิดข้อผิดพลาดในการเข้าสู่ระบบ", error = ex.Message });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var employeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (employeeId == 0)
                    return Unauthorized();

                var result = await _authService.ChangePasswordAsync(employeeId, changePasswordDto);
                if (!result)
                    return BadRequest(new { message = "รหัสผ่านเดิมไม่ถูกต้อง" });

                return Ok(new { message = "เปลี่ยนรหัสผ่านสำเร็จ" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "เกิดข้อผิดพลาดในการเปลี่ยนรหัสผ่าน", error = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<EmployeeDto>> GetCurrentUser()
        {
            try
            {
                var employeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (employeeId == 0)
                    return Unauthorized();

                var employee = await _authService.GetCurrentUserAsync(employeeId);
                if (employee == null)
                    return NotFound();

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "เกิดข้อผิดพลาดในการดึงข้อมูลผู้ใช้", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // ในกรณีของ JWT ไม่จำเป็นต้องทำอะไรใน server
            // แค่ลบ token ใน client side
            return Ok(new { message = "ออกจากระบบสำเร็จ" });
        }

        [HttpPost("signup")]
        public async Task<ActionResult<SignupResponseDto>> Signup(SignupDto signupDto)
        {
            try
            {
                var result = await _authService.SignupAsync(signupDto);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new SignupResponseDto
                {
                    Success = false,
                    Message = "เกิดข้อผิดพลาดในการลงทะเบียน: " + ex.Message
                });
            }
        }

        // ... methods อื่นๆ คงเดิม ...
    }
}

