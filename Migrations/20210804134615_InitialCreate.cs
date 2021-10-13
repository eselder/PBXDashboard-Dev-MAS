using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PBXDashboard.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrentCalls",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    Format = table.Column<string>(nullable: true),
                    FromCallerIdName = table.Column<string>(nullable: true),
                    ToCallerIdName = table.Column<string>(nullable: true),
                    FromCallerIdNumber = table.Column<string>(nullable: true),
                    ToCallerIdNumber = table.Column<string>(nullable: true),
                    PBXID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentCalls", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Queues",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Extension = table.Column<string>(nullable: true),
                    Strategy = table.Column<string>(nullable: true),
                    AccountID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queues", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ReportID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportString = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ReportID);
                });

            migrationBuilder.CreateTable(
                name: "TalkTimeRecords",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    TalkingDuration = table.Column<int>(nullable: false),
                    TotalOutgoingCalls = table.Column<int>(nullable: false),
                    TotalIncomingCalls = table.Column<int>(nullable: false),
                    TotalCalls = table.Column<int>(nullable: false),
                    CallDuration = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalkTimeRecords", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Calls",
                columns: table => new
                {
                    CallID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PBXID = table.Column<long>(nullable: false),
                    UniqueID = table.Column<string>(nullable: true),
                    Origination = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    From = table.Column<string>(nullable: true),
                    FromAccountID = table.Column<int>(nullable: false),
                    ToAccountID = table.Column<int>(nullable: false),
                    FromName = table.Column<string>(nullable: true),
                    FromNumber = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true),
                    ToNumber = table.Column<string>(nullable: true),
                    TotalDuration = table.Column<int>(nullable: false),
                    TalkDuration = table.Column<int>(nullable: false),
                    CdrCallID = table.Column<int>(nullable: false),
                    QueueID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calls", x => x.CallID);
                    table.ForeignKey(
                        name: "FK_Calls_Queues_QueueID",
                        column: x => x.QueueID,
                        principalTable: "Queues",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Extensions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastName = table.Column<string>(nullable: true),
                    Display = table.Column<string>(nullable: true),
                    HasAdditionalPhones = table.Column<bool>(nullable: false),
                    TypeDisplay = table.Column<string>(nullable: true),
                    AccountID = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    MemberCount = table.Column<int>(nullable: false),
                    Strategy = table.Column<string>(nullable: true),
                    CallQueueName = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    QueueID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Extensions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Extensions_Queues_QueueID",
                        column: x => x.QueueID,
                        principalTable: "Queues",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Display = table.Column<string>(nullable: true),
                    CallID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventID);
                    table.ForeignKey(
                        name: "FK_Events_Calls_CallID",
                        column: x => x.CallID,
                        principalTable: "Calls",
                        principalColumn: "CallID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QueueLogs",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QueueAccountID = table.Column<string>(nullable: true),
                    QueueExtension = table.Column<string>(nullable: true),
                    EnterPosition = table.Column<int>(nullable: false),
                    MemberAccountID = table.Column<string>(nullable: true),
                    WaitTime = table.Column<int>(nullable: false),
                    QueueName = table.Column<string>(nullable: true),
                    MemberExtension = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    UniqueID = table.Column<string>(nullable: true),
                    MemberName = table.Column<string>(nullable: true),
                    TalkTime = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    MemberMisses = table.Column<int>(nullable: false),
                    CallID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QueueLogs_Calls_CallID",
                        column: x => x.CallID,
                        principalTable: "Calls",
                        principalColumn: "CallID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calls_QueueID",
                table: "Calls",
                column: "QueueID");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CallID",
                table: "Events",
                column: "CallID");

            migrationBuilder.CreateIndex(
                name: "IX_Extensions_QueueID",
                table: "Extensions",
                column: "QueueID");

            migrationBuilder.CreateIndex(
                name: "IX_QueueLogs_CallID",
                table: "QueueLogs",
                column: "CallID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrentCalls");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Extensions");

            migrationBuilder.DropTable(
                name: "QueueLogs");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "TalkTimeRecords");

            migrationBuilder.DropTable(
                name: "Calls");

            migrationBuilder.DropTable(
                name: "Queues");
        }
    }
}
