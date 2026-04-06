using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartManBackend.Migrations
{
    /// <inheritdoc />
    public partial class ReservationSourceAndExternalSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalSourceReservationId",
                table: "reservations",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_reservations_RoomId_Source_ExternalSourceReservationId",
                table: "reservations",
                columns: new[] { "RoomId", "Source", "ExternalSourceReservationId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_reservations_RoomId_Source_ExternalSourceReservationId",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "ExternalSourceReservationId",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "reservations");
        }
    }
}
