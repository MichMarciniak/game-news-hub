using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class IndexSearchEventsAndGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS fuzzystrmatch;");
            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS ix_games_title_trgm 
                ON "Games" USING GIN("Title" gin_trgm_ops);
                """);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP EXTENSION IF EXISTS fuzzystrmatch;");
            migrationBuilder.Sql(
                """
                DROP INDEX IF EXISTS ix_games_title_trgm 
                ON "Games" USING GIN("Title" gin_trgm_ops);
                """);

        }
    }
}
