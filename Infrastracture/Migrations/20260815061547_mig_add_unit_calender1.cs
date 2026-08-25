using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class migaddunitcalender1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitQuotaPersons_UnitQuotaPersons_UnitQuotaPersonId",
                table: "UnitQuotaPersons");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitQuotaPersons_UnitQuotas_UnitQuotaId1",
                table: "UnitQuotaPersons");

            migrationBuilder.DropIndex(
                name: "IX_UnitQuotaPersons_UnitQuotaId",
                table: "UnitQuotaPersons");

            migrationBuilder.DropIndex(
                name: "IX_UnitQuotaPersons_UnitQuotaId1",
                table: "UnitQuotaPersons");

            migrationBuilder.DropIndex(
                name: "IX_UnitQuotaPersons_UnitQuotaPersonId",
                table: "UnitQuotaPersons");

            migrationBuilder.DropIndex(
                name: "IX_UnitQuotaPersons_UnitStatisticId_DayType_MealId_NationalCode",
                table: "UnitQuotaPersons");

            migrationBuilder.DropIndex(
                name: "IX_UnitQuotaPersons_UnitStatisticId_DayType_MealId_PersonId",
                table: "UnitQuotaPersons");

            migrationBuilder.DropColumn(
                name: "UnitQuotaId1",
                table: "UnitQuotaPersons");

            migrationBuilder.DropColumn(
                name: "UnitQuotaPersonId",
                table: "UnitQuotaPersons");

            migrationBuilder.CreateTable(
                name: "UnitCalendars",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrgId = table.Column<int>(type: "int", nullable: false),
                    OrgTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CalendarDate = table.Column<DateTime>(type: "date", nullable: false),
                    DayType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitCalendars", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotaPersons_UnitQuotaId_MealId_NationalCode",
                table: "UnitQuotaPersons",
                columns: new[] { "UnitQuotaId", "MealId", "NationalCode" },
                unique: true,
                filter: "[NationalCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotaPersons_UnitQuotaId_MealId_PersonCode",
                table: "UnitQuotaPersons",
                columns: new[] { "UnitQuotaId", "MealId", "PersonCode" },
                unique: true,
                filter: "[PersonCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotaPersons_UnitQuotaId_MealId_PersonId",
                table: "UnitQuotaPersons",
                columns: new[] { "UnitQuotaId", "MealId", "PersonId" },
                unique: true,
                filter: "[PersonId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotaPersons_UnitStatisticId",
                table: "UnitQuotaPersons",
                column: "UnitStatisticId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitCalendars_OrgId_CalendarDate",
                table: "UnitCalendars",
                columns: new[] { "OrgId", "CalendarDate" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitCalendars");

            migrationBuilder.DropIndex(
                name: "IX_UnitQuotaPersons_UnitQuotaId_MealId_NationalCode",
                table: "UnitQuotaPersons");

            migrationBuilder.DropIndex(
                name: "IX_UnitQuotaPersons_UnitQuotaId_MealId_PersonCode",
                table: "UnitQuotaPersons");

            migrationBuilder.DropIndex(
                name: "IX_UnitQuotaPersons_UnitQuotaId_MealId_PersonId",
                table: "UnitQuotaPersons");

            migrationBuilder.DropIndex(
                name: "IX_UnitQuotaPersons_UnitStatisticId",
                table: "UnitQuotaPersons");

            migrationBuilder.AddColumn<long>(
                name: "UnitQuotaId1",
                table: "UnitQuotaPersons",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UnitQuotaPersonId",
                table: "UnitQuotaPersons",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotaPersons_UnitQuotaId",
                table: "UnitQuotaPersons",
                column: "UnitQuotaId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotaPersons_UnitQuotaId1",
                table: "UnitQuotaPersons",
                column: "UnitQuotaId1");

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotaPersons_UnitQuotaPersonId",
                table: "UnitQuotaPersons",
                column: "UnitQuotaPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotaPersons_UnitStatisticId_DayType_MealId_NationalCode",
                table: "UnitQuotaPersons",
                columns: new[] { "UnitStatisticId", "DayType", "MealId", "NationalCode" },
                unique: true,
                filter: "[NationalCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotaPersons_UnitStatisticId_DayType_MealId_PersonId",
                table: "UnitQuotaPersons",
                columns: new[] { "UnitStatisticId", "DayType", "MealId", "PersonId" },
                unique: true,
                filter: "[PersonId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitQuotaPersons_UnitQuotaPersons_UnitQuotaPersonId",
                table: "UnitQuotaPersons",
                column: "UnitQuotaPersonId",
                principalTable: "UnitQuotaPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitQuotaPersons_UnitQuotas_UnitQuotaId1",
                table: "UnitQuotaPersons",
                column: "UnitQuotaId1",
                principalTable: "UnitQuotas",
                principalColumn: "Id");
        }
    }
}
