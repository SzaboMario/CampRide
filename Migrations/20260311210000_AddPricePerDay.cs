using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampRide.Migrations
{
    /// <inheritdoc />
    public partial class AddPricePerDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PricePerDay",
                table: "Caravans",
                type: "REAL",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerDay",
                table: "Caravans");
        }
    }
}
