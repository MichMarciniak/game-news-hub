using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class IndexNormalizedNameUsingGIN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "CREATE INDEX IF NOT EXISTS ix_events_normalizedname_trgm " +
                "ON \"Events\" USING GIN (\"NormalizedName\" gin_trgm_ops);"
            );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP INDEX IF EXISTS ix_events_normalizedname_trgm " +
                "ON \"Events\" USING GIN (\"NormalizedName\" gin_trgm_ops);"
            );

        }
    }
}
