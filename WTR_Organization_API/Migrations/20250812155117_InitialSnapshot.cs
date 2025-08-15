using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WTR_Organization_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Empty - ไม่ทำอะไรเพราะ Tables มีอยู่ในฐานข้อมูลแล้ว
            // This migration is just to sync EF with existing database structure
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Empty - ไม่ต้อง drop tables เพราะไม่ได้สร้างอะไร
        }
    }
}