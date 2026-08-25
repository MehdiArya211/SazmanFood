using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class migeditdininghall1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "DiningHall",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<long>(
                name: "KitchenId",
                table: "DiningHall",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerName",
                table: "DiningHall",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "DiningHall",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SupplyType",
                table: "DiningHall",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsageType",
                table: "DiningHall",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DiningHall_KitchenId",
                table: "DiningHall",
                column: "KitchenId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiningHall_Kitchens_KitchenId",
                table: "DiningHall",
                column: "KitchenId",
                principalTable: "Kitchens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiningHall_Kitchens_KitchenId",
                table: "DiningHall");

            migrationBuilder.DropIndex(
                name: "IX_DiningHall_KitchenId",
                table: "DiningHall");

            migrationBuilder.DropColumn(
                name: "KitchenId",
                table: "DiningHall");

            migrationBuilder.DropColumn(
                name: "ManagerName",
                table: "DiningHall");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "DiningHall");

            migrationBuilder.DropColumn(
                name: "SupplyType",
                table: "DiningHall");

            migrationBuilder.DropColumn(
                name: "UsageType",
                table: "DiningHall");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "DiningHall",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }
    }
}
