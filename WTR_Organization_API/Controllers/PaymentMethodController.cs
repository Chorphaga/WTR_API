using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WTROrganization.DTOs;
using WTROrganization.Services;

namespace WTROrganization.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodService;

        public PaymentMethodController(IPaymentMethodService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
        }

        /// <summary>
        /// ดึงรายการวิธีการชำระเงินทั้งหมด
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetPaymentMethods()
        {
            try
            {
                var paymentMethods = await _paymentMethodService.GetAllPaymentMethodsAsync();
                return Ok(paymentMethods);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการดึงข้อมูลวิธีการชำระเงิน",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ดึงวิธีการชำระเงินตาม ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentMethodDto>> GetPaymentMethod(int id)
        {
            try
            {
                var paymentMethod = await _paymentMethodService.GetPaymentMethodByIdAsync(id);

                if (paymentMethod == null)
                    return NotFound(new { message = "ไม่พบวิธีการชำระเงินที่ระบุ" });

                return Ok(paymentMethod);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการดึงข้อมูลวิธีการชำระเงิน",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ดึงวิธีการชำระเงินตามรหัส
        /// </summary>
        [HttpGet("by-code/{code}")]
        public async Task<ActionResult<PaymentMethodDto>> GetPaymentMethodByCode(string code)
        {
            try
            {
                var paymentMethod = await _paymentMethodService.GetPaymentMethodByCodeAsync(code);

                if (paymentMethod == null)
                    return NotFound(new { message = "ไม่พบวิธีการชำระเงินที่ระบุ" });

                return Ok(paymentMethod);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการดึงข้อมูลวิธีการชำระเงิน",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// เพิ่มวิธีการชำระเงินใหม่
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PaymentMethodDto>> CreatePaymentMethod(CreatePaymentMethodDto createPaymentMethodDto)
        {
            try
            {
                // ตรวจสอบว่ารหัสซ้ำหรือไม่
                var existingMethod = await _paymentMethodService.GetPaymentMethodByCodeAsync(createPaymentMethodDto.MethodCode);
                if (existingMethod != null)
                {
                    return BadRequest(new { message = "รหัสวิธีการชำระเงินนี้มีอยู่แล้ว" });
                }

                var paymentMethod = await _paymentMethodService.CreatePaymentMethodAsync(createPaymentMethodDto);
                return CreatedAtAction(nameof(GetPaymentMethod), new { id = paymentMethod.PaymentMethodId }, paymentMethod);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการเพิ่มวิธีการชำระเงิน",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// อัพเดทวิธีการชำระเงิน
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PaymentMethodDto>> UpdatePaymentMethod(int id, CreatePaymentMethodDto updatePaymentMethodDto)
        {
            try
            {
                // ตรวจสอบว่ารหัสซ้ำหรือไม่ (ยกเว้นตัวเอง)
                var existingMethod = await _paymentMethodService.GetPaymentMethodByCodeAsync(updatePaymentMethodDto.MethodCode);
                if (existingMethod != null && existingMethod.PaymentMethodId != id)
                {
                    return BadRequest(new { message = "รหัสวิธีการชำระเงินนี้มีอยู่แล้ว" });
                }

                var paymentMethod = await _paymentMethodService.UpdatePaymentMethodAsync(id, updatePaymentMethodDto);

                if (paymentMethod == null)
                    return NotFound(new { message = "ไม่พบวิธีการชำระเงินที่ต้องการแก้ไข" });

                return Ok(paymentMethod);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการแก้ไขวิธีการชำระเงิน",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ลบวิธีการชำระเงิน
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentMethod(int id)
        {
            try
            {
                var result = await _paymentMethodService.DeletePaymentMethodAsync(id);

                if (!result)
                    return NotFound(new { message = "ไม่พบวิธีการชำระเงินที่ต้องการลบ" });

                return Ok(new { message = "ลบวิธีการชำระเงินสำเร็จ" });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการลบวิธีการชำระเงิน",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// เพิ่มวิธีการชำระเงินเริ่มต้น
        /// </summary>
        [HttpPost("seed-default")]
        public async Task<IActionResult> SeedDefaultPaymentMethods()
        {
            try
            {
                var defaultMethods = new List<CreatePaymentMethodDto>
                {
                    new CreatePaymentMethodDto { MethodName = "เงินสด", MethodCode = "CASH" },
                    new CreatePaymentMethodDto { MethodName = "โอนเงิน", MethodCode = "TRANSFER" },
                    new CreatePaymentMethodDto { MethodName = "เช็ค", MethodCode = "CHEQUE" },
                    new CreatePaymentMethodDto { MethodName = "บัตรเครดิต", MethodCode = "CREDIT_CARD" }
                };

                var createdMethods = new List<PaymentMethodDto>();

                foreach (var method in defaultMethods)
                {
                    // ตรวจสอบว่ามีอยู่แล้วหรือไม่
                    var existing = await _paymentMethodService.GetPaymentMethodByCodeAsync(method.MethodCode);
                    if (existing == null)
                    {
                        var created = await _paymentMethodService.CreatePaymentMethodAsync(method);
                        createdMethods.Add(created);
                    }
                }

                return Ok(new
                {
                    message = $"เพิ่มวิธีการชำระเงินเริ่มต้นสำเร็จ {createdMethods.Count} รายการ",
                    data = createdMethods
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการเพิ่มวิธีการชำระเงินเริ่มต้น",
                    error = ex.Message
                });
            }
        }
    }
}