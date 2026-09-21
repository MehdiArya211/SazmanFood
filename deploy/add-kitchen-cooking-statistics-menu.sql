/*
 اجرای یک‌باره روی دیتابیس موجود.
 منو فقط برای نقش ادمین (RoleId = 1) ساخته می‌شود.
*/
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @MenuId BIGINT;

SELECT @MenuId = Id
FROM Menus
WHERE Area = N'FoodPlan'
  AND Controller = N'KitchenCookingStatistics'
  AND Action = N'Index'
  AND IsDeleted = 0;

IF @MenuId IS NULL
BEGIN
    SELECT @MenuId = ISNULL(MAX(Id), 0) + 1 FROM Menus;

    INSERT INTO Menus
        (Id, Title, Area, Controller, Action, Sort, ParentId, MaterialIcon,
         ShowInMenu, IsEnabled, HasLink, NeedReAuthorize, CreateDate, IsDeleted)
    VALUES
        (@MenuId, N'آمار پخت آشپزخانه', N'FoodPlan',
         N'KitchenCookingStatistics', N'Index', 6, NULL, N'restaurant',
         1, 1, 1, 0, GETDATE(), 0);
END;

IF NOT EXISTS (SELECT 1 FROM RoleMenus WHERE RoleId = 1 AND MenuId = @MenuId)
    INSERT INTO RoleMenus (RoleId, MenuId) VALUES (1, @MenuId);

COMMIT TRANSACTION;
