using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // スキーマを作成
            string schemaName = TaskFlowDbContext.SchemaName;
            migrationBuilder.Sql($@"CREATE SCHEMA IF NOT EXISTS ""{schemaName}"";");

            // データベースのデフォルトのスキーマを設定
            migrationBuilder.Sql($@"
DO $$
DECLARE dbname text := current_database();
BEGIN
    EXECUTE format(
        'ALTER DATABASE %I SET search_path TO ""{schemaName}"", ""public""',
        dbname
    );
END $$;
");

            // 既存テーブルへの権限設定
            string appUser = TaskFlowDbContext.AppUser;
            string migrationUser = TaskFlowDbContext.MigrationUser;
            migrationBuilder.Sql($@"GRANT USAGE ON SCHEMA ""{schemaName}"" TO ""{appUser}"";");
            migrationBuilder.Sql($@"GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA ""{schemaName}"" TO ""{appUser}"";");
            migrationBuilder.Sql($@"GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA ""{schemaName}"" TO ""{appUser}"";");

            // 新規テーブルへの権限設定
            migrationBuilder.Sql($@"ALTER DEFAULT PRIVILEGES FOR ROLE ""{migrationUser}"" IN SCHEMA ""{schemaName}"" GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO ""{appUser}"";");
            migrationBuilder.Sql($@"ALTER DEFAULT PRIVILEGES FOR ROLE ""{migrationUser}"" IN SCHEMA ""{schemaName}"" GRANT USAGE, SELECT ON SEQUENCES TO ""{appUser}"";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"DROP SCHEMA IF EXISTS ""{TaskFlowDbContext.SchemaName}"";");
        }
    }
}
