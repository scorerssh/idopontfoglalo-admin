using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartManBackend.Migrations
{
    /// <inheritdoc />
    public partial class ReservationAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GuidId",
                table: "rooms",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "reservations",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "reservations",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "reservations",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "reservations",
                type: "longtext",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_rooms_GuidId",
                table: "rooms",
                column: "GuidId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_rooms_GuidId",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "GuidId",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "reservations");
        }
    }
}
