using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwayAPI.Migrations
{
    /// <inheritdoc />
    public partial class removedflagfromleagues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Flag",
                table: "Leagues");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Flag",
                table: "Leagues",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
