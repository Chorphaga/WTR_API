using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WTROrganization.DTOs;
using WTROrganization.Services;

namespace WTROrganization.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BillsController : ControllerBase
    {
        private readonly IBillService _billService;

        public BillsController(IBillService billService)
        {
            _billService = billService;
        }

        /// <summary>
        /// ดึงรายการบิลทั้งหมด
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillDto>>> GetBills()
        {
            try
            {
                var bills = await _billService.GetAllBillsAsync();
                return Ok(bills);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการดึงข้อมูลบิล",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ดึงบิลตาม ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<BillDto>> GetBill(int id)
        {
            try
            {
                var bill = await _billService.GetBillByIdAsync(id);

                if (bill == null)
                    return NotFound(new { message = "ไม่พบบิลที่ระบุ" });

                return Ok(bill);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการดึงข้อมูลบิล",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// สร้างบิลใหม่
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BillDto>> CreateBill(CreateBillDto createBillDto)
        {
            try
            {
                var bill = await _billService.CreateBillAsync(createBillDto);
                return CreatedAtAction(nameof(GetBill), new { id = bill.BillId }, bill);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการสร้างบิล",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// อัพเดทสถานะบิล
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult<BillDto>> UpdateBillStatus(int id, [FromBody] string status)
        {
            try
            {
                var validStatuses = new[] { "รอชำระ", "ชำระแล้ว", "ยกเลิก", "รอส่งสินค้า", "ส่งสินค้าแล้ว" };

                if (!validStatuses.Contains(status))
                    return BadRequest(new { message = "สถานะไม่ถูกต้อง" });

                var bill = await _billService.UpdateBillStatusAsync(id, status);

                if (bill == null)
                    return NotFound(new { message = "ไม่พบบิลที่ต้องการแก้ไข" });

                return Ok(bill);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการอัพเดทสถานะบิล",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// อัพเดทข้อมูลการชำระเงิน
        /// </summary>
        [HttpPatch("{id}/payment")]
        public async Task<ActionResult<BillDto>> UpdateBillPayment(int id, UpdateBillPaymentDto paymentDto)
        {
            try
            {
                var bill = await _billService.UpdateBillPaymentAsync(id, paymentDto);

                if (bill == null)
                    return NotFound(new { message = "ไม่พบบิลที่ต้องการแก้ไข" });

                return Ok(bill);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการอัพเดทข้อมูลการชำระเงิน",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// อัพเดทอัตราภาษีมูลค่าเพิ่ม (VAT)
        /// </summary>
        [HttpPatch("{id}/vat")]
        public async Task<ActionResult<BillDto>> UpdateBillVat(int id, UpdateBillVatDto vatDto)
        {
            try
            {
                if (vatDto.VatRate < 0 || vatDto.VatRate > 100)
                    return BadRequest(new { message = "อัตราภาษีต้องอยู่ระหว่าง 0-100%" });

                var bill = await _billService.UpdateBillVatAsync(id, vatDto);

                if (bill == null)
                    return NotFound(new { message = "ไม่พบบิลที่ต้องการแก้ไข" });

                return Ok(bill);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการอัพเดทอัตราภาษี",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// อัพเดทข้อมูลบุคคลที่เกี่ยวข้อง
        /// </summary>
        [HttpPatch("{id}/people")]
        public async Task<ActionResult<BillDto>> UpdateBillPeople(int id, UpdateBillPeopleDto peopleDto)
        {
            try
            {
                // สามารถเพิ่มการ validate ข้อมูลได้ตรงนี้
                var bill = await _billService.GetBillByIdAsync(id);

                if (bill == null)
                    return NotFound(new { message = "ไม่พบบิลที่ต้องการแก้ไข" });

                // อัพเดทข้อมูลผ่าน service (ต้องเพิ่ม method ใน IBillService)
                // var updatedBill = await _billService.UpdateBillPeopleAsync(id, peopleDto);

                return Ok(bill);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการอัพเดทข้อมูลบุคคลที่เกี่ยวข้อง",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// คำนวณยอดรวมของบิลใหม่
        /// </summary>
        [HttpPost("{id}/recalculate")]
        public async Task<ActionResult<BillDto>> RecalculateBillTotals(int id)
        {
            try
            {
                var bill = await _billService.RecalculateBillTotalsAsync(id);

                if (bill == null)
                    return NotFound(new { message = "ไม่พบบิลที่ต้องการคำนวณ" });

                return Ok(bill);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการคำนวณยอดรวมบิล",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ลบบิล (Soft Delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            try
            {
                var result = await _billService.DeleteBillAsync(id);

                if (!result)
                    return NotFound(new { message = "ไม่พบบิลที่ต้องการลบ" });

                return Ok(new { message = "ลบบิลสำเร็จ" });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการลบบิล",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ค้นหาบิลตามเงื่อนไขต่างๆ
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BillDto>>> SearchBills(
            [FromQuery] string? customerName = null,
            [FromQuery] string? billStatus = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? employeeId = null)
        {
            try
            {
                var bills = await _billService.GetAllBillsAsync();

                var filteredBills = bills.AsQueryable();

                if (!string.IsNullOrWhiteSpace(customerName))
                {
                    filteredBills = filteredBills.Where(b =>
                        b.CustomerName != null &&
                        b.CustomerName.Contains(customerName, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(billStatus))
                {
                    filteredBills = filteredBills.Where(b => b.BillStatus == billStatus);
                }

                if (fromDate.HasValue)
                {
                    filteredBills = filteredBills.Where(b => b.CreateDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    filteredBills = filteredBills.Where(b => b.CreateDate <= toDate.Value);
                }

                if (employeeId.HasValue)
                {
                    filteredBills = filteredBills.Where(b => b.EmployeeId == employeeId.Value);
                }

                return Ok(filteredBills.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการค้นหาบิล",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ดึงสถิติบิลรายเดือน
        /// </summary>
        [HttpGet("monthly-stats")]
        public async Task<ActionResult> GetMonthlyBillStats([FromQuery] int year = 0, [FromQuery] int month = 0)
        {
            try
            {
                if (year == 0) year = DateTime.Now.Year;
                if (month == 0) month = DateTime.Now.Month;

                var bills = await _billService.GetAllBillsAsync();
                var monthlyBills = bills.Where(b =>
                    b.CreateDate.Year == year &&
                    b.CreateDate.Month == month).ToList();

                var stats = new
                {
                    Year = year,
                    Month = month,
                    TotalBills = monthlyBills.Count,
                    TotalRevenue = monthlyBills.Sum(b => b.GrandTotal),
                    PaidBills = monthlyBills.Count(b => b.BillStatus == "ชำระแล้ว"),
                    PendingBills = monthlyBills.Count(b => b.BillStatus == "รอชำระ"),
                    CancelledBills = monthlyBills.Count(b => b.BillStatus == "ยกเลิก"),
                    AverageOrderValue = monthlyBills.Any() ? monthlyBills.Average(b => b.GrandTotal) : 0,
                    PaymentMethods = monthlyBills
                        .GroupBy(b => b.PaymentMethod ?? "ไม่ระบุ")
                        .Select(g => new { Method = g.Key, Count = g.Count(), Total = g.Sum(b => b.GrandTotal) })
                        .ToList()
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการดึงสถิติบิลรายเดือน",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ดึงบิลที่ครบกำหนดชำระ
        /// </summary>
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<BillDto>>> GetOverdueBills()
        {
            try
            {
                var bills = await _billService.GetAllBillsAsync();
                var overdueBills = bills.Where(b =>
                    b.DueDate.HasValue &&
                    b.DueDate.Value < DateTime.Now &&
                    b.PaymentStatus != "ชำระแล้ว").ToList();

                return Ok(overdueBills);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการดึงบิลที่ครบกำหนด",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ส่งออกข้อมูลบิลเป็น CSV
        /// </summary>
        [HttpGet("export")]
        public async Task<ActionResult> ExportBillsToCSV(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var bills = await _billService.GetAllBillsAsync();

                if (fromDate.HasValue)
                    bills = bills.Where(b => b.CreateDate >= fromDate.Value);

                if (toDate.HasValue)
                    bills = bills.Where(b => b.CreateDate <= toDate.Value);

                var csvContent = GenerateBillCSV(bills.ToList());
                var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);

                return File(bytes, "text/csv", $"bills_export_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "เกิดข้อผิดพลาดในการส่งออกข้อมูล",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Helper method สำหรับสร้าง CSV
        /// </summary>
        private string GenerateBillCSV(List<BillDto> bills)
        {
            var csv = new System.Text.StringBuilder();

            // Header
            csv.AppendLine("BillId,InvoiceNumber,BillType,CustomerName,EmployeeName,CreateDate,DueDate,SubTotal,VatAmount,GrandTotal,BillStatus,PaymentStatus,PaymentMethod");

            // Data
            foreach (var bill in bills)
            {
                csv.AppendLine($"{bill.BillId}," +
                              $"{bill.InvoiceNumber}," +
                              $"{bill.BillType}," +
                              $"{bill.CustomerName}," +
                              $"{bill.EmployeeName}," +
                              $"{bill.CreateDate:yyyy-MM-dd}," +
                              $"{bill.DueDate?.ToString("yyyy-MM-dd")}," +
                              $"{bill.SubTotal}," +
                              $"{bill.VatAmount}," +
                              $"{bill.GrandTotal}," +
                              $"{bill.BillStatus}," +
                              $"{bill.PaymentStatus}," +
                              $"{bill.PaymentMethod}");
            }

            return csv.ToString();
        }
    }
}