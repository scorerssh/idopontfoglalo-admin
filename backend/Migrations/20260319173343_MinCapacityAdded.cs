using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartManBackend.Migrations
{
    /// <inheritdoc />
    public partial class MinCapacityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinCapacity",
                table: "rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinCapacity",
                table: "rooms");
        }
    }
}
