using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class migaddexterafoodperson1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuestExtraFoodRequestPersons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuestExtraFoodRequestId = table.Column<long>(type: "bigint", nullable: false),
                    OrgId = table.Column<int>(type: "int", nullable: false),
                    MealId = table.Column<long>(type: "bigint", nullable: false),
                    PersonalTypeId = table.Column<long>(type: "bigint", nullable: false),
                    DiningHallId = table.Column<long>(type: "bigint", nullable: true),
                    PersonId = table.Column<long>(type: "bigint", nullable: true),
                    PersonCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NationalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RankTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    CreatorFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestExtraFoodRequestPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuestExtraFoodRequestPersons_DiningHall_DiningHallId",
                        column: x => x.DiningHallId,
                        principalTable: "DiningHall",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuestExtraFoodRequestPersons_GuestExtraFoodRequests_GuestExtraFoodRequestId",
                        column: x => x.GuestExtraFoodRequestId,
                        principalTable: "GuestExtraFoodRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuestExtraFoodRequestPersons_DiningHallId",
                table: "GuestExtraFoodRequestPersons",
                column: "DiningHallId");

            migrationBuilder.CreateIndex(
                name: "IX_GuestExtraFoodRequestPersons_GuestExtraFoodRequestId_NationalCode",
                table: "GuestExtraFoodRequestPersons",
                columns: new[] { "GuestExtraFoodRequestId", "NationalCode" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [NationalCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GuestExtraFoodRequestPersons_GuestExtraFoodRequestId_PersonCode",
                table: "GuestExtraFoodRequestPersons",
                columns: new[] { "GuestExtraFoodRequestId", "PersonCode" },
                unique: true,
                filter: "[IsDeleted] = 0 AND [PersonCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuestExtraFoodRequestPersons");
        }
    }
}
