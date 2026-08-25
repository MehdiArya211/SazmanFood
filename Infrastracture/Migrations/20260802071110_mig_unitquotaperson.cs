using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class migunitquotaperson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnitQuotaPersons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitStatisticId = table.Column<long>(type: "bigint", nullable: false),
                    UnitQuotaId = table.Column<long>(type: "bigint", nullable: false),
                    OrgId = table.Column<int>(type: "int", nullable: false),
                    DayType = table.Column<int>(type: "int", nullable: false),
                    MealId = table.Column<long>(type: "bigint", nullable: false),
                    PersonalTypeId = table.Column<long>(type: "bigint", nullable: false),
                    DiningHallId = table.Column<long>(type: "bigint", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: true),
                    PersonCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NationalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RankTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    CreatorFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnitQuotaId1 = table.Column<long>(type: "bigint", nullable: true),
                    UnitQuotaPersonId = table.Column<long>(type: "bigint", nullable: true),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitQuotaPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitQuotaPersons_DiningHall_DiningHallId",
                        column: x => x.DiningHallId,
                        principalTable: "DiningHall",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitQuotaPersons_Meal_MealId",
                        column: x => x.MealId,
                        principalTable: "Meal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitQuotaPersons_PersonalType_PersonalTypeId",
                        column: x => x.PersonalTypeId,
                        principalTable: "PersonalType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitQuotaPersons_UnitQuotaPersons_UnitQuotaPersonId",
                        column: x => x.UnitQuotaPersonId,
                        principalTable: "UnitQuotaPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitQuotaPersons_UnitQuotas_UnitQuotaId",
                        column: x => x.UnitQuotaId,
                        principalTable: "UnitQuotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitQuotaPersons_UnitQuotas_UnitQuotaId1",
                        column: x => x.UnitQuotaId1,
                        principalTable: "UnitQuotas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UnitQuotaPersons_UnitStatistics_UnitStatisticId",
                        column: x => x.UnitStatisticId,
                        principalTable: "UnitStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotaPersons_DiningHallId",
                table: "UnitQuotaPersons",
                column: "DiningHallId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotaPersons_MealId",
                table: "UnitQuotaPersons",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitQuotaPersons_PersonalTypeId",
                table: "UnitQuotaPersons",
                column: "PersonalTypeId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitQuotaPersons");
        }
    }
}
