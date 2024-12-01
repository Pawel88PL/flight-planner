using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFlightPlanRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightPlanRequests");

            migrationBuilder.CreateTable(
                name: "FlightPlanResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartureICAO = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    ArrivalICAO = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    DepartureTime = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    FlightDay = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    FlightDuration = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    AircraftId = table.Column<int>(type: "int", nullable: false),
                    DepartureMETAR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureTAF = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArrivalMETAR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArrivalTAF = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightPlanResponses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightPlanResponses");

            migrationBuilder.CreateTable(
                name: "FlightPlanRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AircraftId = table.Column<int>(type: "int", nullable: false),
                    ArrivalICAO = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    ArrivalMETAR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArrivalTAF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartureICAO = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    DepartureMETAR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartureTAF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartureTime = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    FetchWeatherData = table.Column<bool>(type: "bit", nullable: false),
                    FlightDay = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    FlightDuration = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightPlanRequests", x => x.Id);
                });
        }
    }
}
