using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AIResponse_FlightPlanIdNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIResponses_FlightPlans_FlightPlanId1",
                table: "AIResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_FlightPlans_AIResponses_AIResponseId",
                table: "FlightPlans");

            migrationBuilder.DropIndex(
                name: "IX_FlightPlans_AIResponseId",
                table: "FlightPlans");

            migrationBuilder.DropIndex(
                name: "IX_AIResponses_FlightPlanId1",
                table: "AIResponses");

            migrationBuilder.DropColumn(
                name: "AIResponseId",
                table: "FlightPlans");

            migrationBuilder.DropColumn(
                name: "FlightPlanId1",
                table: "AIResponses");

            migrationBuilder.CreateIndex(
                name: "IX_AIResponses_FlightPlanId",
                table: "AIResponses",
                column: "FlightPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_AIResponses_FlightPlans_FlightPlanId",
                table: "AIResponses",
                column: "FlightPlanId",
                principalTable: "FlightPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIResponses_FlightPlans_FlightPlanId",
                table: "AIResponses");

            migrationBuilder.DropIndex(
                name: "IX_AIResponses_FlightPlanId",
                table: "AIResponses");

            migrationBuilder.AddColumn<int>(
                name: "AIResponseId",
                table: "FlightPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FlightPlanId1",
                table: "AIResponses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FlightPlans_AIResponseId",
                table: "FlightPlans",
                column: "AIResponseId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_FlightPlans_AIResponses_AIResponseId",
                table: "FlightPlans",
                column: "AIResponseId",
                principalTable: "AIResponses",
                principalColumn: "Id");
        }
    }
}
