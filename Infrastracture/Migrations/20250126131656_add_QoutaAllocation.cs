using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class addQoutaAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganGarrisonType",
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
                    table.PrimaryKey("PK_OrganGarrisonType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganGarrison",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<long>(type: "bigint", nullable: false),
                    OrganGarrisonTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: true),
                    SortName = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganGarrison", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganGarrison_OrganGarrisonType_OrganGarrisonTypeId",
                        column: x => x.OrganGarrisonTypeId,
                        principalTable: "OrganGarrisonType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganGarrison_OrganGarrison_ParentId",
                        column: x => x.ParentId,
                        principalTable: "OrganGarrison",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QoutaAllocations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganGarrisonId = table.Column<long>(type: "bigint", nullable: true),
                    OrgId = table.Column<long>(type: "bigint", nullable: true),
                    OrgTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    QoutaAllocationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DayId = table.Column<long>(type: "bigint", nullable: false),
                    DayTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MealId = table.Column<long>(type: "bigint", nullable: false),
                    MealTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QoutaAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QoutaAllocations_Days_DayId",
                        column: x => x.DayId,
                        principalTable: "Days",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QoutaAllocations_Meal_MealId",
                        column: x => x.MealId,
                        principalTable: "Meal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QoutaAllocations_OrganGarrison_OrganGarrisonId",
                        column: x => x.OrganGarrisonId,
                        principalTable: "OrganGarrison",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Statistic",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganGarrisorId = table.Column<long>(type: "bigint", nullable: false),
                    OfficerCount = table.Column<int>(type: "int", nullable: true),
                    SolidierCount = table.Column<int>(type: "int", nullable: true),
                    OrganGarrisonId = table.Column<long>(type: "bigint", nullable: true),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statistic_OrganGarrison_OrganGarrisonId",
                        column: x => x.OrganGarrisonId,
                        principalTable: "OrganGarrison",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QoutaPersons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PersonalCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    QoutaAllocationId = table.Column<long>(type: "bigint", nullable: false),
                    OrgId = table.Column<long>(type: "bigint", nullable: false),
                    OrgTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PersonalTypeId = table.Column<long>(type: "bigint", nullable: false),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QoutaPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QoutaPersons_PersonalType_PersonalTypeId",
                        column: x => x.PersonalTypeId,
                        principalTable: "PersonalType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QoutaPersons_QoutaAllocations_QoutaAllocationId",
                        column: x => x.QoutaAllocationId,
                        principalTable: "QoutaAllocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganGarrison_OrganGarrisonTypeId",
                table: "OrganGarrison",
                column: "OrganGarrisonTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganGarrison_ParentId",
                table: "OrganGarrison",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_QoutaAllocations_DayId",
                table: "QoutaAllocations",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_QoutaAllocations_MealId",
                table: "QoutaAllocations",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_QoutaAllocations_OrganGarrisonId",
                table: "QoutaAllocations",
                column: "OrganGarrisonId");

            migrationBuilder.CreateIndex(
                name: "IX_QoutaPersons_PersonalTypeId",
                table: "QoutaPersons",
                column: "PersonalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QoutaPersons_QoutaAllocationId",
                table: "QoutaPersons",
                column: "QoutaAllocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Statistic_OrganGarrisonId",
                table: "Statistic",
                column: "OrganGarrisonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QoutaPersons");

            migrationBuilder.DropTable(
                name: "Statistic");

            migrationBuilder.DropTable(
                name: "QoutaAllocations");

            migrationBuilder.DropTable(
                name: "OrganGarrison");

            migrationBuilder.DropTable(
                name: "OrganGarrisonType");
        }
    }
}
