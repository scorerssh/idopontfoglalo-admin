using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartManBackend.Migrations
{
    /// <inheritdoc />
    public partial class Priceadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "rooms");
        }
    }
}
