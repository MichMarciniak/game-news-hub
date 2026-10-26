using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWishlistTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_AspNetUsers_AppUserId1",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_AppUserId1",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "AppUserId1",
                table: "Games");

            migrationBuilder.CreateTable(
                name: "UserWishlistGames",
                columns: table => new
                {
                    AppUser1Id = table.Column<int>(type: "integer", nullable: false),
                    WishlistedGamesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWishlistGames", x => new { x.AppUser1Id, x.WishlistedGamesId });
                    table.ForeignKey(
                        name: "FK_UserWishlistGames_AspNetUsers_AppUser1Id",
                        column: x => x.AppUser1Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWishlistGames_Games_WishlistedGamesId",
                        column: x => x.WishlistedGamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserWishlistGames_WishlistedGamesId",
                table: "UserWishlistGames",
                column: "WishlistedGamesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserWishlistGames");

            migrationBuilder.AddColumn<int>(
                name: "AppUserId1",
                table: "Games",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_AppUserId1",
                table: "Games",
                column: "AppUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_AspNetUsers_AppUserId1",
                table: "Games",
                column: "AppUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
