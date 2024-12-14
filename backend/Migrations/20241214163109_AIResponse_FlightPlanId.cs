using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AIResponse_FlightPlanId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FlightPlanId",
                table: "AIResponses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FlightPlanId1",
                table: "AIResponses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AIResponses_FlightPlanId1",
                table: "AIResponses",
                column: "FlightPlanId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AIResponses_FlightPlans_FlightPlanId1",
                table: "AIResponses",
                column: "FlightPlanId1",
                principalTable: "FlightPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIResponses_FlightPlans_FlightPlanId1",
                table: "AIResponses");

            migrationBuilder.DropIndex(
                name: "IX_AIResponses_FlightPlanId1",
                table: "AIResponses");

            migrationBuilder.DropColumn(
                name: "FlightPlanId",
                table: "AIResponses");

            migrationBuilder.DropColumn(
                name: "FlightPlanId1",
                table: "AIResponses");
        }
    }
}
