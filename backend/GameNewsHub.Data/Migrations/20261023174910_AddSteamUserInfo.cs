using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSteamUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId1",
                table: "Games",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<long[]>(
                name: "FriendsSteamIds",
                table: "AspNetUsers",
                type: "bigint[]",
                nullable: false,
                defaultValue: new long[0]);

            migrationBuilder.AddColumn<long>(
                name: "SteamId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SteamIdAuth",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FriendsSteamIds", "SteamId", "SteamIdAuth" },
                values: new object[] { new long[0], null, null });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "FriendsSteamIds",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SteamId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SteamIdAuth",
                table: "AspNetUsers");
        }
    }
}
