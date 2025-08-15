using Microsoft.EntityFrameworkCore;
using System;
using WTROrganization.Data;
using WTROrganization.DTOs;
using WTROrganization.Models;

namespace WTROrganization.Services
{
    public class BillService : IBillService
    {
        private readonly InventoryDbContext _context;

        public BillService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BillDto>> GetAllBillsAsync()
        {
            return await _context.Bills
                .Include(b => b.Employee)
                .Include(b => b.Customer)
                .Include(b => b.ApprovedByEmployee)
                .Include(b => b.BillItems)
                    .ThenInclude(bi => bi.Stock)
                .Include(b => b.BillItems)
                    .ThenInclude(bi => bi.Product)
                .Where(b => b.IsActive)
                .Select(b => new BillDto
                {
                    BillId = b.BillId,
                    BillType = b.BillType,
                    EmployeeId = b.EmployeeId,
                    EmployeeName = $"{b.Employee.FirstName} {b.Employee.LastName}",
                    CustomerId = b.CustomerId,
                    CustomerName = b.Customer.Name,
                    TotalPrice = b.TotalPrice,
                    BillStatus = b.BillStatus,
                    Remark = b.Remark,
                    CreateDate = b.CreateDate,

                    // ฟิลด์ใหม่
                    PaymentMethod = b.PaymentMethod,
                    PaymentStatus = b.PaymentStatus,
                    DueDate = b.DueDate,
                    VatRate = b.VatRate,
                    VatAmount = b.VatAmount,
                    SubTotal = b.SubTotal,
                    GrandTotal = b.GrandTotal,
                    InvoiceNumber = b.InvoiceNumber,
                    ApprovedBy = b.ApprovedBy,
                    ApprovedByName = b.ApprovedByEmployee != null ?
                        $"{b.ApprovedByEmployee.FirstName} {b.ApprovedByEmployee.LastName}" : null,
                    ReceivedBy = b.ReceivedBy,
                    CheckedBy = b.CheckedBy,
                    DeliveryBy = b.DeliveryBy,
                    PaymentTerms = b.PaymentTerms,

                    BillItems = b.BillItems.Select(bi => new BillItemDto
                    {
                        BillItemId = bi.BillItemId,
                        BillId = bi.BillId,
                        ItemId = bi.ItemId,
                        ItemName = bi.Stock != null ? bi.Stock.ItemName : null,
                        ProductId = bi.ProductId,
                        ProductName = bi.Product != null ? bi.Product.ProductName : null,
                        Quantity = bi.Quantity,
                        PricePerUnit = bi.PricePerUnit,
                        TotalPrice = bi.TotalPrice
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<BillDto?> GetBillByIdAsync(int id)
        {
            var bill = await _context.Bills
                .Include(b => b.Employee)
                .Include(b => b.Customer)
                .Include(b => b.ApprovedByEmployee)
                .Include(b => b.BillItems)
                    .ThenInclude(bi => bi.Stock)
                .Include(b => b.BillItems)
                    .ThenInclude(bi => bi.Product)
                .Where(b => b.BillId == id && b.IsActive)
                .FirstOrDefaultAsync();

            if (bill == null) return null;

            return new BillDto
            {
                BillId = bill.BillId,
                BillType = bill.BillType,
                EmployeeId = bill.EmployeeId,
                EmployeeName = $"{bill.Employee.FirstName} {bill.Employee.LastName}",
                CustomerId = bill.CustomerId,
                CustomerName = bill.Customer.Name,
                TotalPrice = bill.TotalPrice,
                BillStatus = bill.BillStatus,
                Remark = bill.Remark,
                CreateDate = bill.CreateDate,

                // ฟิลด์ใหม่
                PaymentMethod = bill.PaymentMethod,
                PaymentStatus = bill.PaymentStatus,
                DueDate = bill.DueDate,
                VatRate = bill.VatRate,
                VatAmount = bill.VatAmount,
                SubTotal = bill.SubTotal,
                GrandTotal = bill.GrandTotal,
                InvoiceNumber = bill.InvoiceNumber,
                ApprovedBy = bill.ApprovedBy,
                ApprovedByName = bill.ApprovedByEmployee != null ?
                    $"{bill.ApprovedByEmployee.FirstName} {bill.ApprovedByEmployee.LastName}" : null,
                ReceivedBy = bill.ReceivedBy,
                CheckedBy = bill.CheckedBy,
                DeliveryBy = bill.DeliveryBy,
                PaymentTerms = bill.PaymentTerms,

                BillItems = bill.BillItems.Select(bi => new BillItemDto
                {
                    BillItemId = bi.BillItemId,
                    BillId = bi.BillId,
                    ItemId = bi.ItemId,
                    ItemName = bi.Stock != null ? bi.Stock.ItemName : null,
                    ProductId = bi.ProductId,
                    ProductName = bi.Product != null ? bi.Product.ProductName : null,
                    Quantity = bi.Quantity,
                    PricePerUnit = bi.PricePerUnit,
                    TotalPrice = bi.TotalPrice
                }).ToList()
            };
        }

        public async Task<BillDto> CreateBillAsync(CreateBillDto createBillDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // คำนวณยอดรวมจากรายการสินค้า
                decimal subTotal = createBillDto.BillItems.Sum(item => item.Quantity * item.PricePerUnit);

                // คำนวณ VAT
                decimal vatRate = createBillDto.VatRate ?? 0.00m;
                decimal vatAmount = subTotal * (vatRate / 100);
                decimal grandTotal = subTotal + vatAmount;

                // Generate Invoice Number ถ้าไม่ได้ส่งมา
                string invoiceNumber = string.IsNullOrEmpty(createBillDto.InvoiceNumber)
                    ? await GenerateInvoiceNumberAsync()
                    : createBillDto.InvoiceNumber;

                // สร้างบิลใหม่
                var bill = new Bill
                {
                    BillType = createBillDto.BillType,
                    EmployeeId = createBillDto.EmployeeId,
                    CustomerId = createBillDto.CustomerId,
                    BillStatus = createBillDto.BillStatus ?? "รอชำระ",
                    Remark = createBillDto.Remark,

                    // ฟิลด์ใหม่
                    PaymentMethod = createBillDto.PaymentMethod ?? "CASH",
                    PaymentStatus = createBillDto.PaymentStatus ?? "รอชำระ",
                    DueDate = createBillDto.DueDate,
                    VatRate = vatRate,
                    VatAmount = vatAmount,
                    SubTotal = subTotal,
                    TotalPrice = subTotal, // Keep for backward compatibility
                    GrandTotal = grandTotal,
                    InvoiceNumber = invoiceNumber,
                    ApprovedBy = createBillDto.ApprovedBy,
                    ReceivedBy = createBillDto.ReceivedBy,
                    CheckedBy = createBillDto.CheckedBy,
                    DeliveryBy = createBillDto.DeliveryBy,
                    PaymentTerms = createBillDto.PaymentTerms,

                    IsActive = true,
                    CreateDate = DateTime.Now
                };

                _context.Bills.Add(bill);
                await _context.SaveChangesAsync();

                // เพิ่มรายการสินค้าในบิล
                foreach (var item in createBillDto.BillItems)
                {
                    var billItem = new BillItem
                    {
                        BillId = bill.BillId,
                        ItemId = item.ItemId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        PricePerUnit = item.PricePerUnit,
                        TotalPrice = item.Quantity * item.PricePerUnit,
                        IsActive = true,
                        CreateDate = DateTime.Now
                    };

                    _context.BillItems.Add(billItem);

                    // อัพเดทสต็อก
                    if (item.ItemId.HasValue)
                    {
                        var stock = await _context.Stocks.FindAsync(item.ItemId.Value);
                        if (stock != null)
                        {
                            stock.Amount -= item.Quantity;
                            stock.UpdateDate = DateTime.Now;
                        }
                    }
                    else if (item.ProductId.HasValue)
                    {
                        var product = await _context.Products.FindAsync(item.ProductId.Value);
                        if (product != null)
                        {
                            product.Amount -= item.Quantity;
                            product.UpdateDate = DateTime.Now;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // ดึงข้อมูลบิลที่สร้างเสร็จแล้ว
                return await GetBillByIdAsync(bill.BillId) ?? new BillDto();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<BillDto?> UpdateBillStatusAsync(int id, string status)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null || !bill.IsActive) return null;

            bill.BillStatus = status;
            bill.PaymentStatus = status; // Sync payment status
            bill.UpdateDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return await GetBillByIdAsync(id);
        }

        public async Task<BillDto?> UpdateBillPaymentAsync(int id, UpdateBillPaymentDto paymentDto)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null || !bill.IsActive) return null;

            bill.PaymentMethod = paymentDto.PaymentMethod;
            bill.PaymentStatus = paymentDto.PaymentStatus;
            bill.DueDate = paymentDto.DueDate;
            bill.PaymentTerms = paymentDto.PaymentTerms;
            bill.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return await GetBillByIdAsync(id);
        }

        public async Task<BillDto?> UpdateBillVatAsync(int id, UpdateBillVatDto vatDto)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null || !bill.IsActive) return null;

            // คำนวณ VAT ใหม่
            decimal subTotal = bill.SubTotal == 0 ? 0m : bill.SubTotal;
            decimal vatAmount = subTotal * (vatDto.VatRate / 100);
            decimal grandTotal = subTotal + vatAmount;

            bill.VatRate = vatDto.VatRate;
            bill.VatAmount = vatAmount;
            bill.GrandTotal = grandTotal;
            bill.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return await GetBillByIdAsync(id);
        }

        public async Task<bool> DeleteBillAsync(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null) return false;

            bill.IsActive = false;
            bill.UpdateDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        // Helper method to generate invoice number
        private async Task<string> GenerateInvoiceNumberAsync()
        {
            var today = DateTime.Now;
            var prefix = $"INV-{today:yyMM}";

            var lastInvoice = await _context.Bills
                .Where(b => b.InvoiceNumber != null && b.InvoiceNumber.StartsWith(prefix))
                .OrderByDescending(b => b.InvoiceNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNumber))
            {
                var lastNumber = lastInvoice.InvoiceNumber.Substring(prefix.Length);
                if (int.TryParse(lastNumber, out int parsedNumber))
                {
                    nextNumber = parsedNumber + 1;
                }
            }

            return $"{prefix}{nextNumber:D5}"; // INV-2408-00001
        }

        // Method to recalculate bill totals
        public async Task<BillDto?> RecalculateBillTotalsAsync(int billId)
        {
            var bill = await _context.Bills
                .Include(b => b.BillItems)
                .Where(b => b.BillId == billId && b.IsActive)
                .FirstOrDefaultAsync();

            if (bill == null) return null;

            // คำนวณยอดรวมใหม่
            decimal subTotal = bill.BillItems.Sum(item => item.TotalPrice);
            decimal vatAmount = subTotal * (bill.VatRate / 100);
            decimal grandTotal = subTotal + vatAmount;

            bill.SubTotal = subTotal;
            bill.TotalPrice = subTotal; // Keep for backward compatibility
            bill.VatAmount = vatAmount;
            bill.GrandTotal = grandTotal;
            bill.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return await GetBillByIdAsync(billId);
        }

        public async Task<BillDto?> UpdateBillPeopleAsync(int id, UpdateBillPeopleDto peopleDto)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null || !bill.IsActive) return null;

            bill.ApprovedBy = peopleDto.ApprovedBy;
            bill.ReceivedBy = peopleDto.ReceivedBy;
            bill.CheckedBy = peopleDto.CheckedBy;
            bill.DeliveryBy = peopleDto.DeliveryBy;
            bill.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return await GetBillByIdAsync(id);
        }
        
    }


}