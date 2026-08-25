using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class mig14050510 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitStatisticDetail_PersonalType_PersonalTypeId",
                table: "UnitStatisticDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitStatisticDetail_UnitStatistics_UnitStatisticId",
                table: "UnitStatisticDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitStatisticDetail_YeganType_YeganTypeId",
                table: "UnitStatisticDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitStatistics_UnitStatistics_PreviousId",
                table: "UnitStatistics");

            migrationBuilder.DropIndex(
                name: "IX_UnitStatistics_PreviousId",
                table: "UnitStatistics");

            migrationBuilder.DropIndex(
                name: "IX_FoodSource_YeganTypeId",
                table: "FoodSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnitStatisticDetail",
                table: "UnitStatisticDetail");

            migrationBuilder.DropIndex(
                name: "IX_UnitStatisticDetail_UnitStatisticId",
                table: "UnitStatisticDetail");

            migrationBuilder.DropColumn(
                name: "PreviousId",
                table: "UnitStatistics");

            migrationBuilder.RenameTable(
                name: "UnitStatisticDetail",
                newName: "UnitStatisticDetails");

            migrationBuilder.RenameIndex(
                name: "IX_UnitStatisticDetail_YeganTypeId",
                table: "UnitStatisticDetails",
                newName: "IX_UnitStatisticDetails_YeganTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_UnitStatisticDetail_PersonalTypeId",
                table: "UnitStatisticDetails",
                newName: "IX_UnitStatisticDetails_PersonalTypeId");

            migrationBuilder.AddColumn<int>(
                name: "DayType",
                table: "FoodSource",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnitStatisticDetails",
                table: "UnitStatisticDetails",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UnitQuotas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitStatisticId = table.Column<long>(type: "bigint", nullable: false),
                    OrgId = table.Column<int>(type: "int", nullable: false),
                    OrgTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PersonalTypeId = table.Column<long>(type: "bigint", nullable: false),
                    YeganTypeId = table.Column<long>(type: "bigint", nullable: false),
                    DayType = table.Column<int>(type: "int", nullable: false),
                    FoodSourceId = table.Column<long>(type: "bigint", nullable: false),
                    PersonnelCount = table.Column<int>(type: "int", nullable: false),
                    PercentBreakfast = table.Column<double>(type: "float", nullable: false),
                    BreakfastQuota = table.Column<int>(type: "int", nullable: false),
                    PercentLunch = table.Column<double>(type: "float", nullable: false),
                    LunchQuota = table.Column<int>(type: "int", nullable: false),
                    PercentDinner = table.Column<double>(type: "float", nullable: false),
                    DinnerQuota = table.Column<int>(type: "int", nullable: false),
                    CalculateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitQuotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitQuotas_FoodSource_FoodSourceId",
                        column: x => x.FoodSourceId,
                        principalTable: "FoodSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitQuotas_PersonalType_PersonalTypeId",
                        column: x => x.PersonalTypeId,
                        principalTable: "PersonalType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitQuotas_UnitStatistics_UnitStatisticId",
                        column: x => x.UnitStatisticId,
                        principalTable: "UnitStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitQuotas_YeganType_YeganTypeId",
                        column: x => x.YeganTypeId,
                        principalTable: "YeganType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitStatistics_OrgId",
                table: "UnitStatistics",
                column: "OrgId",
                unique: true,
                filter: "[IsActive] = 1 AND [Status] = 3");

            migrationBuilder.CreateIndex(
                name: "IX_FoodSource_YeganTypeId_PersonalTypeId_DayType",
                table: "FoodSource",
                columns: new[] { "YeganTypeId", "PersonalTypeId", "DayType" });

            migrationBuilder.CreateIndex(
                name: "IX_UnitStatisticDetails_UnitStatisticId_PersonalTypeId_YeganTypeId",
                table: "UnitStatisticDetails",
                columns: new[] { "UnitStatisticId", "PersonalTypeId", "YeganTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotas_FoodSourceId",
                table: "UnitQuotas",
                column: "FoodSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotas_PersonalTypeId",
                table: "UnitQuotas",
                column: "PersonalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotas_UnitStatisticId_PersonalTypeId_YeganTypeId_DayType",
                table: "UnitQuotas",
                columns: new[] { "UnitStatisticId", "PersonalTypeId", "YeganTypeId", "DayType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotas_YeganTypeId",
                table: "UnitQuotas",
                column: "YeganTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitStatisticDetails_PersonalType_PersonalTypeId",
                table: "UnitStatisticDetails",
                column: "PersonalTypeId",
                principalTable: "PersonalType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitStatisticDetails_UnitStatistics_UnitStatisticId",
                table: "UnitStatisticDetails",
                column: "UnitStatisticId",
                principalTable: "UnitStatistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitStatisticDetails_YeganType_YeganTypeId",
                table: "UnitStatisticDetails",
                column: "YeganTypeId",
                principalTable: "YeganType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitStatisticDetails_PersonalType_PersonalTypeId",
                table: "UnitStatisticDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitStatisticDetails_UnitStatistics_UnitStatisticId",
                table: "UnitStatisticDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitStatisticDetails_YeganType_YeganTypeId",
                table: "UnitStatisticDetails");

            migrationBuilder.DropTable(
                name: "UnitQuotas");

            migrationBuilder.DropIndex(
                name: "IX_UnitStatistics_OrgId",
                table: "UnitStatistics");

            migrationBuilder.DropIndex(
                name: "IX_FoodSource_YeganTypeId_PersonalTypeId_DayType",
                table: "FoodSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnitStatisticDetails",
                table: "UnitStatisticDetails");

            migrationBuilder.DropIndex(
                name: "IX_UnitStatisticDetails_UnitStatisticId_PersonalTypeId_YeganTypeId",
                table: "UnitStatisticDetails");

            migrationBuilder.DropColumn(
                name: "DayType",
                table: "FoodSource");

            migrationBuilder.RenameTable(
                name: "UnitStatisticDetails",
                newName: "UnitStatisticDetail");

            migrationBuilder.RenameIndex(
                name: "IX_UnitStatisticDetails_YeganTypeId",
                table: "UnitStatisticDetail",
                newName: "IX_UnitStatisticDetail_YeganTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_UnitStatisticDetails_PersonalTypeId",
                table: "UnitStatisticDetail",
                newName: "IX_UnitStatisticDetail_PersonalTypeId");

            migrationBuilder.AddColumn<long>(
                name: "PreviousId",
                table: "UnitStatistics",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnitStatisticDetail",
                table: "UnitStatisticDetail",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UnitStatistics_PreviousId",
                table: "UnitStatistics",
                column: "PreviousId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodSource_YeganTypeId",
                table: "FoodSource",
                column: "YeganTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitStatisticDetail_UnitStatisticId",
                table: "UnitStatisticDetail",
                column: "UnitStatisticId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitStatisticDetail_PersonalType_PersonalTypeId",
                table: "UnitStatisticDetail",
                column: "PersonalTypeId",
                principalTable: "PersonalType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitStatisticDetail_UnitStatistics_UnitStatisticId",
                table: "UnitStatisticDetail",
                column: "UnitStatisticId",
                principalTable: "UnitStatistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitStatisticDetail_YeganType_YeganTypeId",
                table: "UnitStatisticDetail",
                column: "YeganTypeId",
                principalTable: "YeganType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitStatistics_UnitStatistics_PreviousId",
                table: "UnitStatistics",
                column: "PreviousId",
                principalTable: "UnitStatistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
