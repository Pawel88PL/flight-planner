using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FlightPlanResponse_New_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArrivalAirportName",
                table: "FlightPlanResponses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArrivalCity",
                table: "FlightPlanResponses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArrivalCountry",
                table: "FlightPlanResponses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DepartureAirportName",
                table: "FlightPlanResponses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DepartureCity",
                table: "FlightPlanResponses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DepartureCountry",
                table: "FlightPlanResponses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivalAirportName",
                table: "FlightPlanResponses");

            migrationBuilder.DropColumn(
                name: "ArrivalCity",
                table: "FlightPlanResponses");

            migrationBuilder.DropColumn(
                name: "ArrivalCountry",
                table: "FlightPlanResponses");

            migrationBuilder.DropColumn(
                name: "DepartureAirportName",
                table: "FlightPlanResponses");

            migrationBuilder.DropColumn(
                name: "DepartureCity",
                table: "FlightPlanResponses");

            migrationBuilder.DropColumn(
                name: "DepartureCountry",
                table: "FlightPlanResponses");
        }
    }
}
