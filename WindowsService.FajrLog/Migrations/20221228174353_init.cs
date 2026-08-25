using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WindowsService.FajrLog.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FajrLog",
                columns: table => new
                {
                    logId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    timeStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    timeOccurrence = table.Column<long>(type: "bigint", nullable: false),
                    timeRegister = table.Column<long>(type: "bigint", nullable: false),
                    timeDuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    timeSend = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    timeFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    timeTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    actionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    actionSubType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    actionDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    actionId = table.Column<long>(type: "bigint", nullable: true),
                    actionFlag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    actionSensitivity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    logNum = table.Column<long>(type: "bigint", nullable: true),
                    userName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    consoleUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nationalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    uniqueId = table.Column<long>(type: "bigint", nullable: true),
                    firstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employeeNum = table.Column<long>(type: "bigint", nullable: true),
                    membership = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    accessLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phoneNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    groupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    carNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    formName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    forceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    forceUniqueId = table.Column<long>(type: "bigint", nullable: true),
                    orgName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    orgUniqueId = table.Column<long>(type: "bigint", nullable: true),
                    depName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    depUniqueId = table.Column<long>(type: "bigint", nullable: true),
                    secName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    secUniqueId = table.Column<long>(type: "bigint", nullable: true),
                    partName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    partUniqueId = table.Column<long>(type: "bigint", nullable: true),
                    zoneName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    zoneId = table.Column<long>(type: "bigint", nullable: true),
                    cityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cityId = table.Column<long>(type: "bigint", nullable: true),
                    gateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    subSystemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    appName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    appVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    appId = table.Column<long>(type: "bigint", nullable: true),
                    appVendor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    appServerIP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    appServerHostName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    appPortNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    appDBIP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    appDBName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    clientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    clientUniqueId = table.Column<long>(type: "bigint", nullable: true),
                    MACAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HDDSerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    userAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    deviceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    deviceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    deviceUniqueId = table.Column<long>(type: "bigint", nullable: true),
                    deviceComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    deviceStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    deviceBusType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetSubType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetUniqueId = table.Column<long>(type: "bigint", nullable: true),
                    targetSpec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetIP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetUserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetUserIP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetNatioanalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetEmployeeNum = table.Column<long>(type: "bigint", nullable: true),
                    targetMembership = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetAccessLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetNationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetGender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetSensitivity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetPhoneNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetCarNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetPic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetMACAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetHDDSerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetConsolName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetSubject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetId = table.Column<long>(type: "bigint", nullable: true),
                    targetHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetBusType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetCreateTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetModificationTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetVendor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetMessure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    targetAmount = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FajrLog", x => x.logId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FajrLog");
        }
    }
}
