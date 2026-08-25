using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Constants",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Constants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    MaterialIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowInMenu = table.Column<bool>(type: "bit", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    HasLink = table.Column<bool>(type: "bit", nullable: false),
                    NeedReAuthorize = table.Column<bool>(type: "bit", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Controller = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_Menus_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeriLog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogEvent = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPasswordHistorys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChangDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusSave = table.Column<bool>(type: "bit", nullable: false),
                    ExpPassword = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasswordHistorys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleMenus",
                columns: table => new
                {
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    MenuId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMenus", x => new { x.RoleId, x.MenuId });
                    table.ForeignKey(
                        name: "FK_RoleMenus_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleMenus_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ChangePasswordCycle = table.Column<int>(type: "int", nullable: true),
                    CreatorId = table.Column<long>(type: "bigint", nullable: true),
                    PasswordIsChanged = table.Column<bool>(type: "bit", nullable: false),
                    OmdOrgId = table.Column<int>(type: "int", nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Constants",
                columns: new[] { "Id", "CreateDate", "IsDeleted", "LastEditDate", "LastEditUserId", "RegDate", "RegUserId", "Title", "Type", "Value" },
                values: new object[,]
                {
                    { 1L, new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, null, "تلفن", 0, null },
                    { 2L, new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, null, "ایمیل", 1, null },
                    { 3L, new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, null, "آدرس", 2, null },
                    { 4L, new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, null, "تعداد دفعات مجاز برای ورود ناموفق", 3, "5" },
                    { 5L, new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, null, "اجبار تغییر کلمه عبور کاربر بعد از چند روز؟", 4, "60" },
                    { 6L, new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, null, "تعداد کاراکتر های تکراری مجاز در تغییر کلمه عبور", 5, "4" },
                    { 7L, new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, null, "چک کردن تکراری نبودن پسورد با دو یا سه پسورد قدیم", 6, "3" }
                });

            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "Id", "Action", "Area", "Controller", "CreateDate", "Description", "HasLink", "IsDeleted", "IsEnabled", "LastEditDate", "LastEditUserId", "MaterialIcon", "NeedReAuthorize", "Parameters", "ParentId", "RegDate", "RegUserId", "ShowInMenu", "Sort", "Title" },
                values: new object[,]
                {
                    { 1L, "index", "admin", "dashboard", new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, null, true, null, null, "dashboard", false, null, null, null, null, true, 1, "داشبورد" },
                    { 2L, null, null, null, new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, "account_circle", false, null, null, null, null, true, 7, "امکانات مدیریتی" },
                    { 6L, null, null, null, new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, null, true, null, null, "assignment", false, null, null, null, null, true, 10, "لاگ ها" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreateDate", "Description", "IsDeleted", "IsEnabled", "LastEditDate", "LastEditUserId", "RegDate", "RegUserId", "Title" },
                values: new object[] { 1L, new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, true, null, null, null, null, "ادمین کل" });

            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "Id", "Action", "Area", "Controller", "CreateDate", "Description", "HasLink", "IsDeleted", "IsEnabled", "LastEditDate", "LastEditUserId", "MaterialIcon", "NeedReAuthorize", "Parameters", "ParentId", "RegDate", "RegUserId", "ShowInMenu", "Sort", "Title" },
                values: new object[,]
                {
                    { 3L, "index", "authsystem", "users", new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, null, true, null, null, null, false, null, 2L, null, null, true, 1, "کاربران" },
                    { 4L, "index", "authsystem", "menus", new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, null, true, null, null, null, false, null, 2L, null, null, true, 2, "منو ها" },
                    { 5L, "index", "authsystem", "roles", new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, null, true, null, null, null, false, null, 2L, null, null, true, 3, "نقش ها" },
                    { 7L, "index", "logsystem", "smslogs", new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, null, true, null, null, null, false, null, 6L, null, null, true, 1, "پیام های ارسالی" },
                    { 8L, "index", "logsystem", "loginlogs", new DateTime(2022, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, null, true, null, null, null, false, null, 6L, null, null, true, 3, "ورود و خروج کاربران" },
                    { 9L, "index", "logsystem", "actionlogs", new DateTime(2022, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, null, true, null, null, null, false, null, 6L, null, null, true, 4, "عملیات کاربران" },
                    { 10L, "index", "shared", "constants", new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, null, true, null, null, null, false, null, 2L, null, null, true, 4, "تنظیمات" }
                });

            migrationBuilder.InsertData(
                table: "RoleMenus",
                columns: new[] { "MenuId", "RoleId" },
                values: new object[,]
                {
                    { 1L, 1L },
                    { 2L, 1L },
                    { 6L, 1L }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ChangePasswordCycle", "CityId", "CreateDate", "CreatorId", "IsDeleted", "IsEnabled", "LastEditDate", "LastEditUserId", "Mobile", "Name", "OmdOrgId", "Password", "PasswordIsChanged", "ProvinceId", "RegDate", "RegUserId", "RoleId", "Type", "Username" },
                values: new object[] { 1L, null, null, new DateTime(2022, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, true, null, null, "09123456789", "ادمین", 0, "??+c???V????????????	????{\\8???O??Zn1JM??W?N?O\n????>??ix", false, null, null, null, 1L, 1, "admin" });

            migrationBuilder.InsertData(
                table: "RoleMenus",
                columns: new[] { "MenuId", "RoleId" },
                values: new object[,]
                {
                    { 3L, 1L },
                    { 4L, 1L },
                    { 5L, 1L },
                    { 7L, 1L },
                    { 8L, 1L },
                    { 9L, 1L },
                    { 10L, 1L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Menus_ParentId",
                table: "Menus",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_MenuId",
                table: "RoleMenus",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatorId",
                table: "Users",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Constants");

            migrationBuilder.DropTable(
                name: "RoleMenus");

            migrationBuilder.DropTable(
                name: "SeriLog");

            migrationBuilder.DropTable(
                name: "UserPasswordHistorys");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
