using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class migunitstatistic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitStatisticDetail_UnitStatistic_UnitStatisticId",
                table: "UnitStatisticDetail");

            migrationBuilder.DropTable(
                name: "UnitStatistic");

            migrationBuilder.CreateTable(
                name: "UnitStatistics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrgId = table.Column<int>(type: "int", nullable: false),
                    OrgTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TotalOfficialCount = table.Column<int>(type: "int", nullable: false),
                    TotalDutyCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    CreatorFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApproverId = table.Column<long>(type: "bigint", nullable: true),
                    ApproverFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApproveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PreviousId = table.Column<long>(type: "bigint", nullable: true),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitStatistics_UnitStatistics_PreviousId",
                        column: x => x.PreviousId,
                        principalTable: "UnitStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitStatistics_PreviousId",
                table: "UnitStatistics",
                column: "PreviousId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitStatisticDetail_UnitStatistics_UnitStatisticId",
                table: "UnitStatisticDetail",
                column: "UnitStatisticId",
                principalTable: "UnitStatistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitStatisticDetail_UnitStatistics_UnitStatisticId",
                table: "UnitStatisticDetail");

            migrationBuilder.DropTable(
                name: "UnitStatistics");

            migrationBuilder.CreateTable(
                name: "UnitStatistic",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PreviousId = table.Column<long>(type: "bigint", nullable: true),
                    ApproveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApproverFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApproverId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    OrgId = table.Column<int>(type: "int", nullable: false),
                    OrgTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalDutyCount = table.Column<int>(type: "int", nullable: false),
                    TotalOfficialCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitStatistic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitStatistic_UnitStatistic_PreviousId",
                        column: x => x.PreviousId,
                        principalTable: "UnitStatistic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitStatistic_PreviousId",
                table: "UnitStatistic",
                column: "PreviousId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitStatisticDetail_UnitStatistic_UnitStatisticId",
                table: "UnitStatisticDetail",
                column: "UnitStatisticId",
                principalTable: "UnitStatistic",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
