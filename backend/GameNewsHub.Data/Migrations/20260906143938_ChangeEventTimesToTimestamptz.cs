using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEventTimesToTimestamptz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartTime",
                table: "Events",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EndTime",
                table: "Events",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
                */
            migrationBuilder.Sql(@"
                ALTER TABLE ""Events""
                ALTER COLUMN ""StartTime"" TYPE timestamptz
                USING to_timestamp(""StartTime"");
            ");
            
            migrationBuilder.Sql(@"
                ALTER TABLE ""Events"" 
                ALTER COLUMN ""EndTime"" TYPE timestamptz 
                USING CASE WHEN ""EndTime"" IS NULL THEN NULL ELSE to_timestamp(""EndTime"") END;
            "); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "StartTime",
                table: "Events",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<long>(
                name: "EndTime",
                table: "Events",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
