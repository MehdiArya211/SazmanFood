using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class e1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DaysId",
                table: "OrganGarrisonType",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MealId",
                table: "OrganGarrisonType",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganGarrisonType_DaysId",
                table: "OrganGarrisonType",
                column: "DaysId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganGarrisonType_MealId",
                table: "OrganGarrisonType",
                column: "MealId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganGarrisonType_Days_DaysId",
                table: "OrganGarrisonType",
                column: "DaysId",
                principalTable: "Days",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganGarrisonType_Meal_MealId",
                table: "OrganGarrisonType",
                column: "MealId",
                principalTable: "Meal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganGarrisonType_Days_DaysId",
                table: "OrganGarrisonType");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganGarrisonType_Meal_MealId",
                table: "OrganGarrisonType");

            migrationBuilder.DropIndex(
                name: "IX_OrganGarrisonType_DaysId",
                table: "OrganGarrisonType");

            migrationBuilder.DropIndex(
                name: "IX_OrganGarrisonType_MealId",
                table: "OrganGarrisonType");

            migrationBuilder.DropColumn(
                name: "DaysId",
                table: "OrganGarrisonType");

            migrationBuilder.DropColumn(
                name: "MealId",
                table: "OrganGarrisonType");
        }
    }
}
