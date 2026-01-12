using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwayAPI.Migrations
{
    /// <inheritdoc />
    public partial class removedseasonsfieldfromLeagueDTO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Season",
                table: "Leagues");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Season",
                table: "Leagues",
                type: "integer",
                nullable: true);
        }
    }
}
