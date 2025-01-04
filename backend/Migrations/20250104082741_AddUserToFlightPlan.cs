using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToFlightPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "FlightPlans",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FlightPlans_UserId",
                table: "FlightPlans",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightPlans_AspNetUsers_UserId",
                table: "FlightPlans",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightPlans_AspNetUsers_UserId",
                table: "FlightPlans");

            migrationBuilder.DropIndex(
                name: "IX_FlightPlans_UserId",
                table: "FlightPlans");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FlightPlans");
        }
    }
}
