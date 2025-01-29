using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AkilliEtkinlikPlanlamaApp.Migrations
{
    /// <inheritdoc />
    public partial class AddLatitudeLongitudeToEtkinlikler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Etkinlikler",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Etkinlikler",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Etkinlikler");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Etkinlikler");
        }
    }
}
