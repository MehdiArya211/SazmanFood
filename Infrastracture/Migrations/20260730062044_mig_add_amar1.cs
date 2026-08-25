using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class migaddamar1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnitStatistic",
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
                    table.PrimaryKey("PK_UnitStatistic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitStatistic_UnitStatistic_PreviousId",
                        column: x => x.PreviousId,
                        principalTable: "UnitStatistic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitStatisticDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitStatisticId = table.Column<long>(type: "bigint", nullable: false),
                    PersonalTypeId = table.Column<long>(type: "bigint", nullable: false),
                    YeganTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitStatisticDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitStatisticDetail_PersonalType_PersonalTypeId",
                        column: x => x.PersonalTypeId,
                        principalTable: "PersonalType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitStatisticDetail_UnitStatistic_UnitStatisticId",
                        column: x => x.UnitStatisticId,
                        principalTable: "UnitStatistic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitStatisticDetail_YeganType_YeganTypeId",
                        column: x => x.YeganTypeId,
                        principalTable: "YeganType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitStatistic_PreviousId",
                table: "UnitStatistic",
                column: "PreviousId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitStatisticDetail_PersonalTypeId",
                table: "UnitStatisticDetail",
                column: "PersonalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitStatisticDetail_UnitStatisticId",
                table: "UnitStatisticDetail",
                column: "UnitStatisticId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitStatisticDetail_YeganTypeId",
                table: "UnitStatisticDetail",
                column: "YeganTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitStatisticDetail");

            migrationBuilder.DropTable(
                name: "UnitStatistic");
        }
    }
}
