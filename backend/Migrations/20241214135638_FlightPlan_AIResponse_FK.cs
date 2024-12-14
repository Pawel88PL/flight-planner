using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FlightPlan_AIResponse_FK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightPlans_ArrivalAirports_ArrivalAirportId",
                table: "FlightPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_FlightPlans_DepartureAirports_DepartureAirportId",
                table: "FlightPlans");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightPlans_ArrivalAirports_ArrivalAirportId",
                table: "FlightPlans",
                column: "ArrivalAirportId",
                principalTable: "ArrivalAirports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightPlans_DepartureAirports_DepartureAirportId",
                table: "FlightPlans",
                column: "DepartureAirportId",
                principalTable: "DepartureAirports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightPlans_ArrivalAirports_ArrivalAirportId",
                table: "FlightPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_FlightPlans_DepartureAirports_DepartureAirportId",
                table: "FlightPlans");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightPlans_ArrivalAirports_ArrivalAirportId",
                table: "FlightPlans",
                column: "ArrivalAirportId",
                principalTable: "ArrivalAirports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightPlans_DepartureAirports_DepartureAirportId",
                table: "FlightPlans",
                column: "DepartureAirportId",
                principalTable: "DepartureAirports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
