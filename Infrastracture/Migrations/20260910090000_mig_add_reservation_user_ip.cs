using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20260910090000_mig_add_reservation_user_ip")]
    public partial class migaddreservationuserip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegisteredIpAddress",
                table: "Users",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisteredIpDate",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisteredIpAddress",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegisteredIpDate",
                table: "Users");
        }
    }
}
