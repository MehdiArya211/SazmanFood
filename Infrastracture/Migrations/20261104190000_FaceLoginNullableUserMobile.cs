using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <summary>
    /// شناسه پرسنلی شماره موبایل نیست. حساب‌هایی که از دستگاه
    /// تشخیص چهره ایجاد می‌شوند نباید با شماره جعلی ثبت شوند.
    /// </summary>
    [DbContext(typeof(ApplicationContext))]
    [Migration("20261104190000_FaceLoginNullableUserMobile")]
    public partial class FaceLoginNullableUserMobile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "Users",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // بازگرداندن NOT NULL بدون داده واقعی، به شماره جعلی
            // یا حذف اطلاعات منجر می‌شود. اگر حساب بدون موبایل وجود داشته باشد
            // باید ابتدا شماره واقعی از صاحب حساب دریافت شود.
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM [Users] WHERE [Mobile] IS NULL)
    THROW 51000, 'Cannot roll back: users without a verified mobile number exist.', 1;
");

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "Users",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldNullable: true);
        }
    }
}
