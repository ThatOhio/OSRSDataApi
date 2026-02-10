using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSRSData.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddBingoTeamConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BingoTeamConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TeamName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TeamNameColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    DateTimeColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BingoTeamConfigs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BingoTeamConfigs_CharacterName",
                table: "BingoTeamConfigs",
                column: "CharacterName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BingoTeamConfigs");
        }
    }
}
