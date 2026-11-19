using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNameToUserAndDeleteFriendSteamIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendSteamIds",
                table: "SteamProfile");

            migrationBuilder.RenameColumn(
                name: "GoogleId",
                table: "AspNetUsers",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AspNetUsers",
                newName: "GoogleId");

            migrationBuilder.AddColumn<string>(
                name: "FriendSteamIds",
                table: "SteamProfile",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
