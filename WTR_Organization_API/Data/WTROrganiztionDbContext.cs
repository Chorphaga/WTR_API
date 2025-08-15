using Microsoft.EntityFrameworkCore;
using WTR_Organization_API.Models;
using WTROrganization.Models;

namespace WTROrganization.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        // DbSets เดิม
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillItem> BillItems { get; set; }

        // DbSets ใหม่
        public DbSet<CompanySettings> CompanySettings { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee Configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);
                entity.Property(e => e.EmployeeId).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.IdCardNumber).IsUnique();
            });

            // Customer Configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
                entity.Property(e => e.CustomerId).ValueGeneratedOnAdd();
            });

            // Stock Configuration
            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasKey(e => e.ItemId);
                entity.Property(e => e.ItemId).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.IsActive);
            });

            // Product Configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.ProductId).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.IsActive);
            });

            // Bill Configuration - อัพเดทเพิ่มเติม
            modelBuilder.Entity<Bill>(entity =>
            {
                entity.HasKey(e => e.BillId);
                entity.Property(e => e.BillId).ValueGeneratedOnAdd();

                // Foreign Key Relationships
                entity.HasOne(b => b.Employee)
                    .WithMany(e => e.Bills)
                    .HasForeignKey(b => b.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Customer)
                    .WithMany(c => c.Bills)
                    .HasForeignKey(b => b.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ใหม่: ความสัมพันธ์กับผู้อนุมัติ
                entity.HasOne(b => b.ApprovedByEmployee)
                    .WithMany()
                    .HasForeignKey(b => b.ApprovedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes เดิม
                entity.HasIndex(e => e.EmployeeId);
                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.BillStatus);
                entity.HasIndex(e => e.CreateDate);

                // Indexes ใหม่
                entity.HasIndex(e => e.PaymentMethod);
                entity.HasIndex(e => e.PaymentStatus);
                entity.HasIndex(e => e.DueDate);
                entity.HasIndex(e => e.InvoiceNumber).IsUnique();
                entity.HasIndex(e => e.GrandTotal);
                entity.HasIndex(e => e.ApprovedBy);
            });

            // BillItem Configuration
            modelBuilder.Entity<BillItem>(entity =>
            {
                entity.HasKey(e => e.BillItemId);
                entity.Property(e => e.BillItemId).ValueGeneratedOnAdd();

                entity.HasOne(bi => bi.Bill)
                    .WithMany(b => b.BillItems)
                    .HasForeignKey(bi => bi.BillId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(bi => bi.Stock)
                    .WithMany(s => s.BillItems)
                    .HasForeignKey(bi => bi.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(bi => bi.Product)
                    .WithMany(p => p.BillItems)
                    .HasForeignKey(bi => bi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.BillId);
                entity.HasIndex(e => e.ItemId);
                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => e.CreateDate);
            });

            // CompanySettings Configuration
            modelBuilder.Entity<CompanySettings>(entity =>
            {
                entity.HasKey(e => e.SettingId);
                entity.Property(e => e.SettingId).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.IsActive);
                entity.Property(e => e.CompanyName).IsRequired();
            });

            // PaymentMethod Configuration
            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.PaymentMethodId);
                entity.Property(e => e.PaymentMethodId).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.MethodCode).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.Property(e => e.MethodName).IsRequired();
                entity.Property(e => e.MethodCode).IsRequired();
            });
        }
    }
}