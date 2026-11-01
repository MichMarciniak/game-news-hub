using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class SeparateSteamProfileFromUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendsSteamIds",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SteamId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "SteamProfileId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SteamProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SteamId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    AppUserId = table.Column<int>(type: "integer", nullable: false),
                    FriendSteamIds = table.Column<string>(type: "text", nullable: false),
                    LastImportedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SteamProfile", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "SteamProfileId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SteamProfileId",
                table: "AspNetUsers",
                column: "SteamProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SteamProfile_SteamId",
                table: "SteamProfile",
                column: "SteamId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SteamProfile_SteamProfileId",
                table: "AspNetUsers",
                column: "SteamProfileId",
                principalTable: "SteamProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SteamProfile_SteamProfileId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "SteamProfile");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SteamProfileId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SteamProfileId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<long[]>(
                name: "FriendsSteamIds",
                table: "AspNetUsers",
                type: "bigint[]",
                nullable: false,
                defaultValue: new long[0]);

            migrationBuilder.AddColumn<string>(
                name: "SteamId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FriendsSteamIds", "SteamId" },
                values: new object[] { new long[0], null });
        }
    }
}
