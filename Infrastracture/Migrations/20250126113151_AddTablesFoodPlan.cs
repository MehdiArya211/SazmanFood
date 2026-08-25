using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesFoodPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Days",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    SortName = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Days", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meal",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    SortName = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Season",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    SortName = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Season", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Years",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    SortName = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Years", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodPlan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YearsId = table.Column<long>(type: "bigint", nullable: false),
                    SeasonId = table.Column<long>(type: "bigint", nullable: false),
                    OrganId = table.Column<int>(type: "int", nullable: false),
                    OrganTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodPlan_Season_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Season",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FoodPlan_Years_YearsId",
                        column: x => x.YearsId,
                        principalTable: "Years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FoodPlanDay",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodPlanId = table.Column<long>(type: "bigint", nullable: false),
                    DayId = table.Column<long>(type: "bigint", nullable: false),
                    MealId = table.Column<long>(type: "bigint", nullable: false),
                    FoodId = table.Column<long>(type: "bigint", nullable: false),
                    FoodDesserId = table.Column<long>(type: "bigint", nullable: true),
                    FoodDorchinId = table.Column<long>(type: "bigint", nullable: true),
                    Count = table.Column<int>(type: "int", nullable: true),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodPlanDay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodPlanDay_Days_DayId",
                        column: x => x.DayId,
                        principalTable: "Days",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FoodPlanDay_FoodPlan_FoodPlanId",
                        column: x => x.FoodPlanId,
                        principalTable: "FoodPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FoodPlanDay_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FoodPlanDay_Meal_MealId",
                        column: x => x.MealId,
                        principalTable: "Meal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlan_SeasonId",
                table: "FoodPlan",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlan_YearsId",
                table: "FoodPlan",
                column: "YearsId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlanDay_DayId",
                table: "FoodPlanDay",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlanDay_FoodId",
                table: "FoodPlanDay",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlanDay_FoodPlanId",
                table: "FoodPlanDay",
                column: "FoodPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlanDay_MealId",
                table: "FoodPlanDay",
                column: "MealId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodPlanDay");

            migrationBuilder.DropTable(
                name: "Days");

            migrationBuilder.DropTable(
                name: "FoodPlan");

            migrationBuilder.DropTable(
                name: "Meal");

            migrationBuilder.DropTable(
                name: "Season");

            migrationBuilder.DropTable(
                name: "Years");
        }
    }
}
