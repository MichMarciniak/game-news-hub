using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventSeries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Events",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "EventSeriesId",
                table: "Events",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Events",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EventSeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSeries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventSeriesId",
                table: "Events",
                column: "EventSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSeries_Name",
                table: "EventSeries",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventSeries_EventSeriesId",
                table: "Events",
                column: "EventSeriesId",
                principalTable: "EventSeries",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventSeries_EventSeriesId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "EventSeries");

            migrationBuilder.DropIndex(
                name: "IX_Events_EventSeriesId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventSeriesId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Events",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);
        }
    }
}
