using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SixMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "plant_location_city",
                table: "Plant");

            migrationBuilder.DropColumn(
                name: "plant_location_country",
                table: "Plant");

            migrationBuilder.DropColumn(
                name: "plant_location_geo",
                table: "Plant");

            migrationBuilder.DropColumn(
                name: "plant_location_pincode",
                table: "Plant");

            migrationBuilder.DropColumn(
                name: "plant_location_state",
                table: "Plant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "plant_location_city",
                table: "Plant",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "plant_location_country",
                table: "Plant",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "plant_location_geo",
                table: "Plant",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "plant_location_pincode",
                table: "Plant",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "plant_location_state",
                table: "Plant",
                type: "longtext",
                nullable: false);
        }
    }
}
