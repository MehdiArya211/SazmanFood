using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class migeditQuataAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FoodPlanDayId",
                table: "QoutaAllocations",
                type: "bigint",
                nullable: true);


            migrationBuilder.AddForeignKey(
                name: "FK_QoutaAllocations_FoodPlanDay_FoodPlanDayId",
                table: "QoutaAllocations",
                column: "FoodPlanDayId",
                principalTable: "FoodPlanDay",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


        }
    }
}
