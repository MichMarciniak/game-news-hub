using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEventStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventGenreWeight_Events_EventId1",
                table: "EventGenreWeight");

            migrationBuilder.DropIndex(
                name: "IX_EventGenreWeight_EventId1",
                table: "EventGenreWeight");

            migrationBuilder.DropColumn(
                name: "EventId1",
                table: "EventGenreWeight");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Events",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "EventId1",
                table: "EventGenreWeight",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventGenreWeight_EventId1",
                table: "EventGenreWeight",
                column: "EventId1");

            migrationBuilder.AddForeignKey(
                name: "FK_EventGenreWeight_Events_EventId1",
                table: "EventGenreWeight",
                column: "EventId1",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
