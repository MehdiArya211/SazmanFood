using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class migaddexterafood1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnitCalendars_OrgId_CalendarDate",
                table: "UnitCalendars");

            migrationBuilder.CreateTable(
                name: "GuestExtraFoodRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrgId = table.Column<int>(type: "int", nullable: false),
                    OrgTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MealId = table.Column<long>(type: "bigint", nullable: false),
                    PersonalTypeId = table.Column<long>(type: "bigint", nullable: false),
                    YeganTypeId = table.Column<long>(type: "bigint", nullable: false),
                    FromDate = table.Column<DateTime>(type: "date", nullable: false),
                    ToDate = table.Column<DateTime>(type: "date", nullable: false),
                    GuestCount = table.Column<int>(type: "int", nullable: false),
                    ExtraQuotaCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    CreatorFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequestCreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SenderId = table.Column<long>(type: "bigint", nullable: true),
                    SenderFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApproverId = table.Column<long>(type: "bigint", nullable: true),
                    ApproverFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApproveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestExtraFoodRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuestExtraFoodRequests_Meal_MealId",
                        column: x => x.MealId,
                        principalTable: "Meal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuestExtraFoodRequests_PersonalType_PersonalTypeId",
                        column: x => x.PersonalTypeId,
                        principalTable: "PersonalType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuestExtraFoodRequests_YeganType_YeganTypeId",
                        column: x => x.YeganTypeId,
                        principalTable: "YeganType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuestExtraFoodRequestAttachments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuestExtraFoodRequestId = table.Column<long>(type: "bigint", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UploaderId = table.Column<long>(type: "bigint", nullable: false),
                    UploaderFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditUserId = table.Column<long>(type: "bigint", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestExtraFoodRequestAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuestExtraFoodRequestAttachments_GuestExtraFoodRequests_GuestExtraFoodRequestId",
                        column: x => x.GuestExtraFoodRequestId,
                        principalTable: "GuestExtraFoodRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitCalendars_OrgId_CalendarDate",
                table: "UnitCalendars",
                columns: new[] { "OrgId", "CalendarDate" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_GuestExtraFoodRequestAttachments_GuestExtraFoodRequestId",
                table: "GuestExtraFoodRequestAttachments",
                column: "GuestExtraFoodRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_GuestExtraFoodRequests_MealId",
                table: "GuestExtraFoodRequests",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_GuestExtraFoodRequests_OrgId_MealId_PersonalTypeId_YeganTypeId_Status_FromDate_ToDate",
                table: "GuestExtraFoodRequests",
                columns: new[] { "OrgId", "MealId", "PersonalTypeId", "YeganTypeId", "Status", "FromDate", "ToDate" });

            migrationBuilder.CreateIndex(
                name: "IX_GuestExtraFoodRequests_PersonalTypeId",
                table: "GuestExtraFoodRequests",
                column: "PersonalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GuestExtraFoodRequests_YeganTypeId",
                table: "GuestExtraFoodRequests",
                column: "YeganTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuestExtraFoodRequestAttachments");

            migrationBuilder.DropTable(
                name: "GuestExtraFoodRequests");

            migrationBuilder.DropIndex(
                name: "IX_UnitCalendars_OrgId_CalendarDate",
                table: "UnitCalendars");

            migrationBuilder.CreateIndex(
                name: "IX_UnitCalendars_OrgId_CalendarDate",
                table: "UnitCalendars",
                columns: new[] { "OrgId", "CalendarDate" },
                unique: true,
                filter: "[IsDeleted] = 0 OR [IsDeleted] IS NULL");
        }
    }
}
