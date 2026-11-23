using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20261123090000_AddReservationReportsMenu")]
    public partial class AddReservationReportsMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1 FROM dbo.Menus WITH (UPDLOCK, HOLDLOCK)
                    WHERE ParentId IS NULL AND Title = N'گزارشات' AND IsDeleted = 0
                )
                BEGIN
                    INSERT INTO dbo.Menus
                        (Title, Sort, MaterialIcon, HasLink, ShowInMenu, IsEnabled, IsDeleted, NeedReAuthorize, CreateDate)
                    VALUES
                        (N'گزارشات', 8, N'assessment', 0, 1, 1, 0, 0, GETDATE());
                END;

                DECLARE @ParentId bigint = (
                    SELECT TOP (1) Id FROM dbo.Menus
                    WHERE ParentId IS NULL AND Title = N'گزارشات' AND IsDeleted = 0
                    ORDER BY Id
                );

                DECLARE @Reports TABLE (Title nvarchar(200), Action nvarchar(40), Sort int);
                INSERT INTO @Reports (Title, Action, Sort) VALUES
                    (N'گزارش روزانه رزرو غذا', N'Daily', 1),
                    (N'گزارش روز هفته و ماه', N'Calendar', 2),
                    (N'گزارش به تفکیک غذا', N'ByFood', 3);

                INSERT INTO dbo.Menus
                    (Title, ParentId, Area, Controller, Action, Sort, HasLink,
                     ShowInMenu, IsEnabled, IsDeleted, NeedReAuthorize, CreateDate)
                SELECT r.Title, @ParentId, N'FoodPlan', N'ReservationReports',
                       r.Action, r.Sort, 1, 1, 1, 0, 0, GETDATE()
                FROM @Reports r
                WHERE NOT EXISTS (
                    SELECT 1 FROM dbo.Menus m
                    WHERE m.Area = N'FoodPlan'
                      AND m.Controller = N'ReservationReports'
                      AND m.Action = r.Action AND m.IsDeleted = 0
                );

                IF EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = 1)
                BEGIN
                    INSERT INTO dbo.RoleMenus (RoleId, MenuId)
                    SELECT 1, m.Id
                    FROM dbo.Menus m
                    WHERE (m.Id = @ParentId OR
                        (m.Area = N'FoodPlan' AND m.Controller = N'ReservationReports'
                         AND m.Action IN (N'Daily', N'Calendar', N'ByFood') AND m.IsDeleted = 0))
                    AND NOT EXISTS (
                        SELECT 1 FROM dbo.RoleMenus rm WHERE rm.RoleId = 1 AND rm.MenuId = m.Id
                    );
                END;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // دسترسی‌های منو ممکن است پس از انتشار به نقش‌های دیگر داده شده باشد؛
            // هنگام بازگشت نسخه، منوها و دسترسی‌های تنظیم‌شده حذف نمی‌شوند.
        }
    }
}
