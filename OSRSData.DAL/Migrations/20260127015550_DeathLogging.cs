using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSRSData.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DeathLogging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeathRecordId",
                table: "LogEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeathRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<int>(type: "integer", nullable: false),
                    Killer = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeathRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_DeathRecordId",
                table: "LogEntries",
                column: "DeathRecordId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LogEntries_DeathRecords_DeathRecordId",
                table: "LogEntries",
                column: "DeathRecordId",
                principalTable: "DeathRecords",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogEntries_DeathRecords_DeathRecordId",
                table: "LogEntries");

            migrationBuilder.DropTable(
                name: "DeathRecords");

            migrationBuilder.DropIndex(
                name: "IX_LogEntries_DeathRecordId",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "DeathRecordId",
                table: "LogEntries");
        }
    }
}
