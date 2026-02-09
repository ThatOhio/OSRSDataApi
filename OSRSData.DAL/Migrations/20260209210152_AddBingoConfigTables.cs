using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSRSData.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddBingoConfigTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BingoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Source = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BingoItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BingoWebhooks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    WebhookUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BingoWebhooks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BingoWebhooks_CharacterName",
                table: "BingoWebhooks",
                column: "CharacterName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BingoItems");

            migrationBuilder.DropTable(
                name: "BingoWebhooks");
        }
    }
}
