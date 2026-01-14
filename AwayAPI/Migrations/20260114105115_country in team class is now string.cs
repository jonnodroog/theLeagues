using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwayAPI.Migrations
{
    /// <inheritdoc />
    public partial class countryinteamclassisnowstring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Country_CountryId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_CountryId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Teams");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Teams",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Teams");

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Teams",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CountryId",
                table: "Teams",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Country_CountryId",
                table: "Teams",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "Id");
        }
    }
}
