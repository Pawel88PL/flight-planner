using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FlightPlanRequest_METAR_TAF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArrivalMETAR",
                table: "FlightPlanRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArrivalTAF",
                table: "FlightPlanRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartureMETAR",
                table: "FlightPlanRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartureTAF",
                table: "FlightPlanRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivalMETAR",
                table: "FlightPlanRequests");

            migrationBuilder.DropColumn(
                name: "ArrivalTAF",
                table: "FlightPlanRequests");

            migrationBuilder.DropColumn(
                name: "DepartureMETAR",
                table: "FlightPlanRequests");

            migrationBuilder.DropColumn(
                name: "DepartureTAF",
                table: "FlightPlanRequests");
        }
    }
}
