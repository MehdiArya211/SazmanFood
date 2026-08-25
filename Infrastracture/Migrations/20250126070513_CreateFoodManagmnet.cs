using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class CreateFoodManagmnet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiningHall",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HallCapacity = table.Column<int>(type: "int", nullable: true),
                    OrgId = table.Column<int>(type: "int", nullable: false),
                    OrgTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_DiningHall", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: true),
                    SortName = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kitchens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CookingCapacity = table.Column<int>(type: "int", nullable: true),
                    OrgId = table.Column<int>(type: "int", nullable: false),
                    OrgTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Kitchens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Foods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FoodTypesId = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: true),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Foods_FoodTypes_FoodTypesId",
                        column: x => x.FoodTypesId,
                        principalTable: "FoodTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Foods_FoodTypesId",
                table: "Foods",
                column: "FoodTypesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiningHall");

            migrationBuilder.DropTable(
                name: "Foods");

            migrationBuilder.DropTable(
                name: "Kitchens");

            migrationBuilder.DropTable(
                name: "FoodTypes");
        }
    }
}
