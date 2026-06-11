using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ApartManBackend.Migrations
{
    /// <inheritdoc />
    public partial class ApartmanSmtpSettingAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "apartman_smtp_settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ApartmanId = table.Column<int>(type: "int", nullable: false),
                    Host = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    Password = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true),
                    SenderEmail = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    SenderName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    UseSsl = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_apartman_smtp_settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_apartman_smtp_settings_apartmans_ApartmanId",
                        column: x => x.ApartmanId,
                        principalTable: "apartmans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_apartman_smtp_settings_ApartmanId",
                table: "apartman_smtp_settings",
                column: "ApartmanId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "apartman_smtp_settings");
        }
    }
}
