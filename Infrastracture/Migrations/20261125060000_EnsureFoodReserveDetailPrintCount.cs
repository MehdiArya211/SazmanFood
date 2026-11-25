using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20261125060000_EnsureFoodReserveDetailPrintCount")]
    public partial class EnsureFoodReserveDetailPrintCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'dbo.FoodReserveDetails', N'PrintCount') IS NULL
    ALTER TABLE [dbo].[FoodReserveDetails] ADD [PrintCount] int NOT NULL CONSTRAINT [DF_FoodReserveDetails_PrintCount] DEFAULT (0);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // شمارنده‌های ثبت‌شده نباید با بازگردانی migration حذف شوند.
        }
    }
}
