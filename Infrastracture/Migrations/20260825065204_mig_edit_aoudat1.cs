using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class migeditaoudat1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancelDate",
                table: "UnitStatistics",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "UnitStatistics",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelerFullName",
                table: "UnitStatistics",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CancelerId",
                table: "UnitStatistics",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDate",
                table: "UnitStatistics",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnReason",
                table: "UnitStatistics",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnerFullName",
                table: "UnitStatistics",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReturnerId",
                table: "UnitStatistics",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelDate",
                table: "UnitStatistics");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "UnitStatistics");

            migrationBuilder.DropColumn(
                name: "CancelerFullName",
                table: "UnitStatistics");

            migrationBuilder.DropColumn(
                name: "CancelerId",
                table: "UnitStatistics");

            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "UnitStatistics");

            migrationBuilder.DropColumn(
                name: "ReturnReason",
                table: "UnitStatistics");

            migrationBuilder.DropColumn(
                name: "ReturnerFullName",
                table: "UnitStatistics");

            migrationBuilder.DropColumn(
                name: "ReturnerId",
                table: "UnitStatistics");
        }
    }
}
