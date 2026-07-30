using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameNewsHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFollowedGenres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPlatforms_Platforms_PlatformsId",
                table: "UserPlatforms");

            migrationBuilder.RenameColumn(
                name: "PlatformsId",
                table: "UserPlatforms",
                newName: "FollowedPlatformsId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPlatforms_PlatformsId",
                table: "UserPlatforms",
                newName: "IX_UserPlatforms_FollowedPlatformsId");

            migrationBuilder.CreateTable(
                name: "UserGenres",
                columns: table => new
                {
                    AppUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    FollowedGenresId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGenres", x => new { x.AppUserId, x.FollowedGenresId });
                    table.ForeignKey(
                        name: "FK_UserGenres_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGenres_Genres_FollowedGenresId",
                        column: x => x.FollowedGenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGenres_FollowedGenresId",
                table: "UserGenres",
                column: "FollowedGenresId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPlatforms_Platforms_FollowedPlatformsId",
                table: "UserPlatforms",
                column: "FollowedPlatformsId",
                principalTable: "Platforms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPlatforms_Platforms_FollowedPlatformsId",
                table: "UserPlatforms");

            migrationBuilder.DropTable(
                name: "UserGenres");

            migrationBuilder.RenameColumn(
                name: "FollowedPlatformsId",
                table: "UserPlatforms",
                newName: "PlatformsId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPlatforms_FollowedPlatformsId",
                table: "UserPlatforms",
                newName: "IX_UserPlatforms_PlatformsId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPlatforms_Platforms_PlatformsId",
                table: "UserPlatforms",
                column: "PlatformsId",
                principalTable: "Platforms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
