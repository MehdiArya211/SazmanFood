using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class migeditaoudat2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancelDate",
                table: "GuestExtraFoodRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "GuestExtraFoodRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelerFullName",
                table: "GuestExtraFoodRequests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CancelerId",
                table: "GuestExtraFoodRequests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDate",
                table: "GuestExtraFoodRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnReason",
                table: "GuestExtraFoodRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnerFullName",
                table: "GuestExtraFoodRequests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReturnerId",
                table: "GuestExtraFoodRequests",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelDate",
                table: "GuestExtraFoodRequests");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "GuestExtraFoodRequests");

            migrationBuilder.DropColumn(
                name: "CancelerFullName",
                table: "GuestExtraFoodRequests");

            migrationBuilder.DropColumn(
                name: "CancelerId",
                table: "GuestExtraFoodRequests");

            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "GuestExtraFoodRequests");

            migrationBuilder.DropColumn(
                name: "ReturnReason",
                table: "GuestExtraFoodRequests");

            migrationBuilder.DropColumn(
                name: "ReturnerFullName",
                table: "GuestExtraFoodRequests");

            migrationBuilder.DropColumn(
                name: "ReturnerId",
                table: "GuestExtraFoodRequests");
        }
    }
}
