using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FlightPlanRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlightPlanRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartureICAO = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    ArrivalICAO = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    DepartureTime = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    FlightDay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightDuration = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    AircraftId = table.Column<int>(type: "int", nullable: false),
                    FetchWeatherData = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightPlanRequests", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightPlanRequests");
        }
    }
}
