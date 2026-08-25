using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class mig11q : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PersonId",
                table: "QoutaPersons",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QoutaPersons_PersonId",
                table: "QoutaPersons",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_QoutaPersons_Person_PersonId",
                table: "QoutaPersons",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QoutaPersons_Person_PersonId",
                table: "QoutaPersons");

            migrationBuilder.DropIndex(
                name: "IX_QoutaPersons_PersonId",
                table: "QoutaPersons");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "QoutaPersons");
        }
    }
}
