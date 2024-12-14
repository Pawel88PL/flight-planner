using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FlightPlan_Removed_ICAO_Codes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightPlanResponses");

            migrationBuilder.CreateTable(
                name: "FlightPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartureTime = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    FlightDay = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    FlightDuration = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AircraftId = table.Column<int>(type: "int", nullable: false),
                    DepartureAirportId = table.Column<int>(type: "int", nullable: false),
                    ArrivalAirportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightPlans_ArrivalAirports_ArrivalAirportId",
                        column: x => x.ArrivalAirportId,
                        principalTable: "ArrivalAirports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightPlans_DepartureAirports_DepartureAirportId",
                        column: x => x.DepartureAirportId,
                        principalTable: "DepartureAirports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightPlans_ArrivalAirportId",
                table: "FlightPlans",
                column: "ArrivalAirportId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightPlans_DepartureAirportId",
                table: "FlightPlans",
                column: "DepartureAirportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightPlans");

            migrationBuilder.CreateTable(
                name: "FlightPlanResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AIJustification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AircraftId = table.Column<int>(type: "int", nullable: false),
                    ArrivalAirportName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArrivalCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArrivalCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArrivalICAO = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    ArrivalMETAR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArrivalTAF = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureAirportName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureICAO = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    DepartureMETAR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureTAF = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureTime = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    FlightDay = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    FlightDuration = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightPlanResponses", x => x.Id);
                });
        }
    }
}
