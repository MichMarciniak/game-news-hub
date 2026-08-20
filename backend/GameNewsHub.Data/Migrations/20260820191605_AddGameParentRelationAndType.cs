using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGameParentRelationAndType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentGameId",
                table: "Games",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentGameIgdbId",
                table: "Games",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Games_ParentGameId",
                table: "Games",
                column: "ParentGameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Games_ParentGameId",
                table: "Games",
                column: "ParentGameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Games_ParentGameId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_ParentGameId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ParentGameId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ParentGameIgdbId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Games");
        }
    }
}
