using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    public partial class AddFoodReserveDetailPrintCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrintCount",
                table: "FoodReserveDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1 FROM (VALUES (8), (9), (10), (11)) AS required(Id)
                    WHERE NOT EXISTS (
                        SELECT 1 FROM dbo.Constants AS c WHERE c.Id = required.Id
                    )
                )
                BEGIN
                    SET IDENTITY_INSERT dbo.Constants ON;

                    IF NOT EXISTS (SELECT 1 FROM dbo.Constants WHERE Id = 8)
                        INSERT INTO dbo.Constants
                            (Id, CreateDate, IsDeleted, Title, Type, Value)
                        VALUES
                            (8, '2022-07-25', 0,
                             N'رزرو آزاد غذا بدون نیاز به سهمیه فردی', 7, N'false');

                    IF NOT EXISTS (SELECT 1 FROM dbo.Constants WHERE Id = 9)
                        INSERT INTO dbo.Constants
                            (Id, CreateDate, IsDeleted, Title, Type, Value)
                        VALUES
                            (9, '2022-07-25', 0,
                             N'زمان‌بندی رزرو و چاپ ژتون غذا', 8, NULL);

                    IF NOT EXISTS (SELECT 1 FROM dbo.Constants WHERE Id = 10)
                        INSERT INTO dbo.Constants
                            (Id, CreateDate, IsDeleted, Title, Type, Value)
                        VALUES
                            (10, '2022-07-25', 0,
                             N'پیام صفحه ورود با رمز', 9, NULL);

                    IF NOT EXISTS (SELECT 1 FROM dbo.Constants WHERE Id = 11)
                        INSERT INTO dbo.Constants
                            (Id, CreateDate, IsDeleted, Title, Type, Value)
                        VALUES
                            (11, '2022-07-25', 0,
                             N'پیام صفحه ورود با چهره', 10, NULL);

                    SET IDENTITY_INSERT dbo.Constants OFF;
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // رکوردهای Constants ممکن است قبل از این Migration وجود داشته باشند.
            // هنگام برگشت Migration نباید تنظیمات کاربران حذف شوند.
            migrationBuilder.DropColumn(
                name: "PrintCount",
                table: "FoodReserveDetails");
        }
    }
}