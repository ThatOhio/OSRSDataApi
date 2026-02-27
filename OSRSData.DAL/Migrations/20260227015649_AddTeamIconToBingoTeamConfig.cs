using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSRSData.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamIconToBingoTeamConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeamIcon",
                table: "BingoTeamConfigs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamIcon",
                table: "BingoTeamConfigs");
        }
    }
}
