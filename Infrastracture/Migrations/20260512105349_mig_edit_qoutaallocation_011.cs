using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class migeditqoutaallocation011 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NationalCode",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OrganGarrisonId",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonCode",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PersonId",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "NationalCode", "OrganGarrisonId", "PersonCode", "PersonId" },
                values: new object[] { null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PersonId",
                table: "Users",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Person_PersonId",
                table: "Users",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Person_PersonId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PersonId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NationalCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OrganGarrisonId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PersonCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Users");
        }
    }
}
