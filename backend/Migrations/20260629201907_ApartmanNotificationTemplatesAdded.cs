using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartManBackend.Migrations
{
    /// <inheritdoc />
    public partial class ApartmanNotificationTemplatesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuestEmailIntroTemplate",
                table: "apartman_smtp_settings",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuestSmsTemplate",
                table: "apartman_smtp_settings",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuestEmailIntroTemplate",
                table: "apartman_smtp_settings");

            migrationBuilder.DropColumn(
                name: "GuestSmsTemplate",
                table: "apartman_smtp_settings");
        }
    }
}
