using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FlightPlan_AIResponseId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AIResponseId",
                table: "FlightPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FlightPlans_AIResponseId",
                table: "FlightPlans",
                column: "AIResponseId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightPlans_AIResponses_AIResponseId",
                table: "FlightPlans",
                column: "AIResponseId",
                principalTable: "AIResponses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightPlans_AIResponses_AIResponseId",
                table: "FlightPlans");

            migrationBuilder.DropIndex(
                name: "IX_FlightPlans_AIResponseId",
                table: "FlightPlans");

            migrationBuilder.DropColumn(
                name: "AIResponseId",
                table: "FlightPlans");
        }
    }
}
