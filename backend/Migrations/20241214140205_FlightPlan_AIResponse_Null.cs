using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FlightPlan_AIResponse_Null : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightPlans_AIResponses_AIResponseId",
                table: "FlightPlans");

            migrationBuilder.AlterColumn<int>(
                name: "AIResponseId",
                table: "FlightPlans",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightPlans_AIResponses_AIResponseId",
                table: "FlightPlans",
                column: "AIResponseId",
                principalTable: "AIResponses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightPlans_AIResponses_AIResponseId",
                table: "FlightPlans");

            migrationBuilder.AlterColumn<int>(
                name: "AIResponseId",
                table: "FlightPlans",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightPlans_AIResponses_AIResponseId",
                table: "FlightPlans",
                column: "AIResponseId",
                principalTable: "AIResponses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
