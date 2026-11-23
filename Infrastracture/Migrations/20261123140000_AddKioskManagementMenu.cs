using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <summary>
    /// ثبت منوی مدیریت کیوسک در فهرست دسترسی‌ها و اعطای دسترسی اولیه به ادمین کل.
    /// </summary>
    [DbContext(typeof(ApplicationContext))]
    [Migration("20261123140000_AddKioskManagementMenu")]
    public sealed class AddKioskManagementMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @MenuId bigint;

SELECT TOP (1) @MenuId = [Id]
FROM [Menus]
WHERE ISNULL([Area], N'') = N''
  AND LOWER(ISNULL([Controller], N'')) = N'kioskmanagement'
  AND LOWER(ISNULL([Action], N'')) = N'index'
ORDER BY [Id];

IF @MenuId IS NULL
BEGIN
    INSERT INTO [Menus]
    (
        [Title], [ParentId], [Sort], [MaterialIcon], [ShowInMenu], [IsEnabled],
        [HasLink], [NeedReAuthorize], [Area], [Controller], [Action], [Parameters],
        [Description], [RegUserId], [LastEditUserId], [RegDate], [LastEditDate],
        [CreateDate], [IsDeleted]
    )
    VALUES
    (
        N'مدیریت کیوسک‌ها و دستگاه‌ها', NULL,
        ISNULL((SELECT MAX([Sort]) FROM [Menus] WHERE [ParentId] IS NULL), 0) + 1,
        N'devices', 1, 1, 1, 0, NULL, N'kioskmanagement', N'index', NULL,
        N'اتصال مرورگر کیوسک و مدیریت اطلاعات دستگاه‌های سرویس چهره',
        NULL, NULL, NULL, NULL, SYSDATETIME(), 0
    );

    SET @MenuId = CONVERT(bigint, SCOPE_IDENTITY());
END;

IF NOT EXISTS (SELECT 1 FROM [RoleMenus] WHERE [RoleId] = 1 AND [MenuId] = @MenuId)
   AND EXISTS (SELECT 1 FROM [Roles] WHERE [Id] = 1)
BEGIN
    INSERT INTO [RoleMenus] ([RoleId], [MenuId]) VALUES (1, @MenuId);
END;

IF EXISTS (SELECT 1 FROM [Menus] WHERE [Id] = @MenuId)
BEGIN
    UPDATE [Menus]
    SET [Title] = N'مدیریت کیوسک‌ها و دستگاه‌ها',
        [ShowInMenu] = 1,
        [IsEnabled] = 1,
        [HasLink] = 1,
        [MaterialIcon] = N'devices',
        [IsDeleted] = 0
    WHERE [Id] = @MenuId;
END;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @MenuId bigint;
SELECT TOP (1) @MenuId = [Id]
FROM [Menus]
WHERE ISNULL([Area], N'') = N''
  AND LOWER(ISNULL([Controller], N'')) = N'kioskmanagement'
  AND LOWER(ISNULL([Action], N'')) = N'index'
ORDER BY [Id];

IF @MenuId IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM [RoleMenus] WHERE [MenuId] = @MenuId AND [RoleId] <> 1)
        THROW 51000, 'Kiosk management menu is assigned to other roles; revoke those assignments before rolling back.', 1;

    DELETE FROM [RoleMenus] WHERE [MenuId] = @MenuId;
    DELETE FROM [Menus] WHERE [Id] = @MenuId;
END;
");
        }
    }
}
