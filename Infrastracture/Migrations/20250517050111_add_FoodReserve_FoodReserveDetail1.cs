using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class addFoodReserveFoodReserveDetail1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "FoodReserves",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    WeekStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WeekEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodReserves", x => x.Id);
                });

       

            migrationBuilder.CreateTable(
                name: "FoodReserveDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodReserveId = table.Column<long>(type: "bigint", nullable: false),
                    DayId = table.Column<long>(type: "bigint", nullable: false),
                    MealId = table.Column<long>(type: "bigint", nullable: false),
                    MainFoodId = table.Column<long>(type: "bigint", nullable: true),
                    DessertId = table.Column<long>(type: "bigint", nullable: true),
                    SideDishId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodReserveDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodReserveDetails_FoodReserves_FoodReserveId",
                        column: x => x.FoodReserveId,
                        principalTable: "FoodReserves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


         
        }
    }
}
