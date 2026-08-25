using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class addestedadfileds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstedadKadr",
                table: "OrganGarrison",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstedadVazife",
                table: "OrganGarrison",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FoodReserveDetails_DayId",
                table: "FoodReserveDetails",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodReserveDetails_DessertId",
                table: "FoodReserveDetails",
                column: "DessertId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodReserveDetails_MainFoodId",
                table: "FoodReserveDetails",
                column: "MainFoodId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodReserveDetails_MealId",
                table: "FoodReserveDetails",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodReserveDetails_SideDishId",
                table: "FoodReserveDetails",
                column: "SideDishId");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodReserveDetails_Days_DayId",
                table: "FoodReserveDetails",
                column: "DayId",
                principalTable: "Days",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodReserveDetails_Foods_DessertId",
                table: "FoodReserveDetails",
                column: "DessertId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodReserveDetails_Foods_MainFoodId",
                table: "FoodReserveDetails",
                column: "MainFoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodReserveDetails_Foods_SideDishId",
                table: "FoodReserveDetails",
                column: "SideDishId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodReserveDetails_Meal_MealId",
                table: "FoodReserveDetails",
                column: "MealId",
                principalTable: "Meal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodReserveDetails_Days_DayId",
                table: "FoodReserveDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodReserveDetails_Foods_DessertId",
                table: "FoodReserveDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodReserveDetails_Foods_MainFoodId",
                table: "FoodReserveDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodReserveDetails_Foods_SideDishId",
                table: "FoodReserveDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodReserveDetails_Meal_MealId",
                table: "FoodReserveDetails");

            migrationBuilder.DropIndex(
                name: "IX_FoodReserveDetails_DayId",
                table: "FoodReserveDetails");

            migrationBuilder.DropIndex(
                name: "IX_FoodReserveDetails_DessertId",
                table: "FoodReserveDetails");

            migrationBuilder.DropIndex(
                name: "IX_FoodReserveDetails_MainFoodId",
                table: "FoodReserveDetails");

            migrationBuilder.DropIndex(
                name: "IX_FoodReserveDetails_MealId",
                table: "FoodReserveDetails");

            migrationBuilder.DropIndex(
                name: "IX_FoodReserveDetails_SideDishId",
                table: "FoodReserveDetails");

            migrationBuilder.DropColumn(
                name: "EstedadKadr",
                table: "OrganGarrison");

            migrationBuilder.DropColumn(
                name: "EstedadVazife",
                table: "OrganGarrison");
        }
    }
}
