using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class migeditqoutaallocation01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PersonalTypeId",
                table: "QoutaPersons",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredDate",
                table: "QoutaPersons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeliveredUserId",
                table: "QoutaPersons",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryCode",
                table: "QoutaPersons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryHash",
                table: "QoutaPersons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "QoutaPersons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DessertId",
                table: "QoutaPersons",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FoodReceiverTypeId",
                table: "QoutaPersons",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FoodTokenTypeId",
                table: "QoutaPersons",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelivered",
                table: "QoutaPersons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "SideDishId",
                table: "QoutaPersons",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowOfficeUserRegister",
                table: "QoutaAllocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowPersonChange",
                table: "QoutaAllocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "GuestCapacity",
                table: "QoutaAllocations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinalized",
                table: "QoutaAllocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ManagementTokenCapacity",
                table: "QoutaAllocations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OfficerCapacity",
                table: "QoutaAllocations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisterDeadline",
                table: "QoutaAllocations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoldierCapacity",
                table: "QoutaAllocations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FoodReceiverType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    SortName = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodReceiverType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodTokenType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    SortName = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodTokenType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QoutaPersons_DessertId",
                table: "QoutaPersons",
                column: "DessertId");

            migrationBuilder.CreateIndex(
                name: "IX_QoutaPersons_FoodReceiverTypeId",
                table: "QoutaPersons",
                column: "FoodReceiverTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QoutaPersons_FoodTokenTypeId",
                table: "QoutaPersons",
                column: "FoodTokenTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QoutaPersons_SideDishId",
                table: "QoutaPersons",
                column: "SideDishId");

            migrationBuilder.AddForeignKey(
                name: "FK_QoutaPersons_FoodReceiverType_FoodReceiverTypeId",
                table: "QoutaPersons",
                column: "FoodReceiverTypeId",
                principalTable: "FoodReceiverType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QoutaPersons_FoodTokenType_FoodTokenTypeId",
                table: "QoutaPersons",
                column: "FoodTokenTypeId",
                principalTable: "FoodTokenType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QoutaPersons_Foods_DessertId",
                table: "QoutaPersons",
                column: "DessertId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QoutaPersons_Foods_SideDishId",
                table: "QoutaPersons",
                column: "SideDishId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QoutaPersons_FoodReceiverType_FoodReceiverTypeId",
                table: "QoutaPersons");

            migrationBuilder.DropForeignKey(
                name: "FK_QoutaPersons_FoodTokenType_FoodTokenTypeId",
                table: "QoutaPersons");

            migrationBuilder.DropForeignKey(
                name: "FK_QoutaPersons_Foods_DessertId",
                table: "QoutaPersons");

            migrationBuilder.DropForeignKey(
                name: "FK_QoutaPersons_Foods_SideDishId",
                table: "QoutaPersons");

            migrationBuilder.DropTable(
                name: "FoodReceiverType");

            migrationBuilder.DropTable(
                name: "FoodTokenType");

            migrationBuilder.DropIndex(
                name: "IX_QoutaPersons_DessertId",
                table: "QoutaPersons");

            migrationBuilder.DropIndex(
                name: "IX_QoutaPersons_FoodReceiverTypeId",
                table: "QoutaPersons");

            migrationBuilder.DropIndex(
                name: "IX_QoutaPersons_FoodTokenTypeId",
                table: "QoutaPersons");

            migrationBuilder.DropIndex(
                name: "IX_QoutaPersons_SideDishId",
                table: "QoutaPersons");

            migrationBuilder.DropColumn(
                name: "DeliveredDate",
                table: "QoutaPersons");

            migrationBuilder.DropColumn(
                name: "DeliveredUserId",
                table: "QoutaPersons");

            migrationBuilder.DropColumn(
                name: "DeliveryCode",
                table: "QoutaPersons");

            migrationBuilder.DropColumn(
                name: "DeliveryHash",
                table: "QoutaPersons");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "QoutaPersons");

            migrationBuilder.DropColumn(
                name: "DessertId",
                table: "QoutaPersons");

            migrationBuilder.DropColumn(
                name: "FoodReceiverTypeId",
                table: "QoutaPersons");

            migrationBuilder.DropColumn(
                name: "FoodTokenTypeId",
                table: "QoutaPersons");

            migrationBuilder.DropColumn(
                name: "IsDelivered",
                table: "QoutaPersons");

            migrationBuilder.DropColumn(
                name: "SideDishId",
                table: "QoutaPersons");

            migrationBuilder.DropColumn(
                name: "AllowOfficeUserRegister",
                table: "QoutaAllocations");

            migrationBuilder.DropColumn(
                name: "AllowPersonChange",
                table: "QoutaAllocations");

            migrationBuilder.DropColumn(
                name: "GuestCapacity",
                table: "QoutaAllocations");

            migrationBuilder.DropColumn(
                name: "IsFinalized",
                table: "QoutaAllocations");

            migrationBuilder.DropColumn(
                name: "ManagementTokenCapacity",
                table: "QoutaAllocations");

            migrationBuilder.DropColumn(
                name: "OfficerCapacity",
                table: "QoutaAllocations");

            migrationBuilder.DropColumn(
                name: "RegisterDeadline",
                table: "QoutaAllocations");

            migrationBuilder.DropColumn(
                name: "SoldierCapacity",
                table: "QoutaAllocations");

            migrationBuilder.AlterColumn<long>(
                name: "PersonalTypeId",
                table: "QoutaPersons",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
