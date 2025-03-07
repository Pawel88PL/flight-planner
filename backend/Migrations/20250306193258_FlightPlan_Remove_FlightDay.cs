using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FlightPlan_Remove_FlightDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlightDay",
                table: "FlightPlans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FlightDay",
                table: "FlightPlans",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");
        }
    }
}
