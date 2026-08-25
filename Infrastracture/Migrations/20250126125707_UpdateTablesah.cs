using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablesah : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonalType",
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
                    table.PrimaryKey("PK_PersonalType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YeganType",
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
                    table.PrimaryKey("PK_YeganType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodSource",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YeganTypeId = table.Column<long>(type: "bigint", nullable: false),
                    PersonalTypeId = table.Column<long>(type: "bigint", nullable: false),
                    PercentBreakfast = table.Column<double>(type: "float", nullable: false),
                    PercentLunch = table.Column<double>(type: "float", nullable: false),
                    PercentDinner = table.Column<double>(type: "float", nullable: false),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodSource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodSource_PersonalType_PersonalTypeId",
                        column: x => x.PersonalTypeId,
                        principalTable: "PersonalType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FoodSource_YeganType_YeganTypeId",
                        column: x => x.YeganTypeId,
                        principalTable: "YeganType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodSource_PersonalTypeId",
                table: "FoodSource",
                column: "PersonalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodSource_YeganTypeId",
                table: "FoodSource",
                column: "YeganTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodSource");

            migrationBuilder.DropTable(
                name: "PersonalType");

            migrationBuilder.DropTable(
                name: "YeganType");
        }
    }
}
