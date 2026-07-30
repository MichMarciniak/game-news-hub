using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameNewsHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class EventIgdbIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Events_IgdbId",
                table: "Events",
                column: "IgdbId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_IgdbId",
                table: "Events");
        }
    }
}
