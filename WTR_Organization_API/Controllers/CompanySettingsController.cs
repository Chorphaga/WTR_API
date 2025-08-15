using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WTROrganization.DTOs;
using WTROrganization.Services;

namespace WTROrganization.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompanySettingsController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanySettingsController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        /// <summary>
        /// ดึงข้อมูลการตั้งค่าบริษัท
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<CompanySettingsDto>> GetCompanySettings()
        {
            try
            {
                var companySettings = await _companyService.GetCompanySettingsAsync();

                if (companySettings == null)
                {
                    // Return default settings if not configured yet
                    return Ok(new CompanySettingsDto
                    {
                        SettingId = 0,
                        CompanyName = "WTR Organization",
                        Address = "",
                        PhoneNumber = "",
                        MobileNumber = "",
                        Email = "",
                        TaxId = "",
                        BankAccount = "",
                        BankName = "",
                        IsActive = true
                    });
                }

                return Ok(companySettings);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการดึงข้อมูลบริษัท",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// อัพเดทข้อมูลการตั้งค่าบริษัท
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CompanySettingsDto>> CreateOrUpdateCompanySettings(CreateCompanySettingsDto createCompanySettingsDto)
        {
            try
            {
                var companySettings = await _companyService.CreateOrUpdateCompanySettingsAsync(createCompanySettingsDto);
                return Ok(companySettings);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการบันทึกข้อมูลบริษัท",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ลบข้อมูลการตั้งค่าบริษัท
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanySettings(int id)
        {
            try
            {
                var result = await _companyService.DeleteCompanySettingsAsync(id);

                if (!result)
                    return NotFound(new { message = "ไม่พบข้อมูลบริษัทที่ต้องการลบ" });

                return Ok(new { message = "ลบข้อมูลบริษัทสำเร็จ" });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการลบข้อมูลบริษัท",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ตรวจสอบสถานะการตั้งค่าบริษัท
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult> GetSetupStatus()
        {
            try
            {
                var companySettings = await _companyService.GetCompanySettingsAsync();

                var isSetup = companySettings != null &&
                             !string.IsNullOrEmpty(companySettings.CompanyName) &&
                             !string.IsNullOrEmpty(companySettings.Address);

                return Ok(new
                {
                    isSetup = isSetup,
                    message = isSetup ? "บริษัทได้ตั้งค่าเรียบร้อยแล้ว" : "ยังไม่ได้ตั้งค่าข้อมูลบริษัท"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการตรวจสอบสถานะ",
                    error = ex.Message
                });
            }
        }
    }
}