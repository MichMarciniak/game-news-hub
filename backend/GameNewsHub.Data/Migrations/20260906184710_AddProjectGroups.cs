using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPlatforms");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Platforms",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "PlatformGroupId",
                table: "Platforms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PlatformGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPlatformGroups",
                columns: table => new
                {
                    AppUserId = table.Column<int>(type: "integer", nullable: false),
                    FollowedPlatformGroupsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlatformGroups", x => new { x.AppUserId, x.FollowedPlatformGroupsId });
                    table.ForeignKey(
                        name: "FK_UserPlatformGroups_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPlatformGroups_PlatformGroups_FollowedPlatformGroupsId",
                        column: x => x.FollowedPlatformGroupsId,
                        principalTable: "PlatformGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Platforms_IgdbId",
                table: "Platforms",
                column: "IgdbId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Platforms_PlatformGroupId",
                table: "Platforms",
                column: "PlatformGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformGroups_Name",
                table: "PlatformGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPlatformGroups_FollowedPlatformGroupsId",
                table: "UserPlatformGroups",
                column: "FollowedPlatformGroupsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Platforms_PlatformGroups_PlatformGroupId",
                table: "Platforms",
                column: "PlatformGroupId",
                principalTable: "PlatformGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Platforms_PlatformGroups_PlatformGroupId",
                table: "Platforms");

            migrationBuilder.DropTable(
                name: "UserPlatformGroups");

            migrationBuilder.DropTable(
                name: "PlatformGroups");

            migrationBuilder.DropIndex(
                name: "IX_Platforms_IgdbId",
                table: "Platforms");

            migrationBuilder.DropIndex(
                name: "IX_Platforms_PlatformGroupId",
                table: "Platforms");

            migrationBuilder.DropColumn(
                name: "PlatformGroupId",
                table: "Platforms");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Platforms",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "UserPlatforms",
                columns: table => new
                {
                    AppUserId = table.Column<int>(type: "integer", nullable: false),
                    FollowedPlatformsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlatforms", x => new { x.AppUserId, x.FollowedPlatformsId });
                    table.ForeignKey(
                        name: "FK_UserPlatforms_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPlatforms_Platforms_FollowedPlatformsId",
                        column: x => x.FollowedPlatformsId,
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPlatforms_FollowedPlatformsId",
                table: "UserPlatforms",
                column: "FollowedPlatformsId");
        }
    }
}
