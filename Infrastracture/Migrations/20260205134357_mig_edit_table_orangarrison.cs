using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class migedittableorangarrison : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MainFoodId",
                table: "QoutaPersons",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QoutaPersons_MainFoodId",
                table: "QoutaPersons",
                column: "MainFoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_QoutaPersons_Foods_MainFoodId",
                table: "QoutaPersons",
                column: "MainFoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QoutaPersons_Foods_MainFoodId",
                table: "QoutaPersons");

            migrationBuilder.DropIndex(
                name: "IX_QoutaPersons_MainFoodId",
                table: "QoutaPersons");

            migrationBuilder.DropColumn(
                name: "MainFoodId",
                table: "QoutaPersons");
        }
    }
}
