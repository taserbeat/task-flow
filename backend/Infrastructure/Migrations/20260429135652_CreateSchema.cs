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
            string schemaName = AppDbContext.SchemaName;
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
            string appUser = AppDbContext.AppUser;
            string migrationUser = AppDbContext.MigrationUser;

            // migrationBuilder.Sql($@"GRANT USAGE ON SCHEMA ""{schemaName}"" TO ""{appUser}"";");
            // migrationBuilder.Sql($@"GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA ""{schemaName}"" TO ""{appUser}"";");
            // migrationBuilder.Sql($@"GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA ""{schemaName}"" TO ""{appUser}"";");

            migrationBuilder.Sql($$"""
DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM pg_roles
        WHERE rolname = '{{appUser}}'
    )
    THEN
        BEGIN
            EXECUTE format(
                'GRANT USAGE ON SCHEMA "%s" TO "%s"',
                '{{schemaName}}',
                '{{appUser}}'
            );
        EXCEPTION
            WHEN insufficient_privilege THEN
                    RAISE NOTICE 'Skipping schema grant to appUser';
        END;

        BEGIN
            EXECUTE format(
                'GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA "%s" TO "%s"',
                '{{schemaName}}',
                '{{appUser}}'
            );
        EXCEPTION
            WHEN insufficient_privilege THEN
                RAISE NOTICE 'Skipping table grant to appUser';
        END;

        BEGIN
            EXECUTE format(
                'GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA "%s" TO "%s"',
                '{{schemaName}}',
                '{{appUser}}'
            );
        EXCEPTION
            WHEN insufficient_privilege THEN
                RAISE NOTICE 'Skipping sequence grant to appUser';
        END;
    END IF;
END
$$;
""");

            // 新規テーブルへの権限設定
            // migrationBuilder.Sql($@"ALTER DEFAULT PRIVILEGES FOR ROLE ""{migrationUser}"" IN SCHEMA ""{schemaName}"" GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO ""{appUser}"";");
            // migrationBuilder.Sql($@"ALTER DEFAULT PRIVILEGES FOR ROLE ""{migrationUser}"" IN SCHEMA ""{schemaName}"" GRANT USAGE, SELECT ON SEQUENCES TO ""{appUser}"";");

            migrationBuilder.Sql($$"""
DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM pg_roles
        WHERE rolname = '{{appUser}}'
    )
    AND EXISTS (
        SELECT 1
        FROM pg_roles
        WHERE rolname = '{{migrationUser}}'
    )
    THEN
        BEGIN
            EXECUTE format(
                'ALTER DEFAULT PRIVILEGES FOR ROLE "%s" IN SCHEMA "%s" GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO "%s"',
                '{{migrationUser}}',
                '{{schemaName}}',
                '{{appUser}}'
            );
        EXCEPTION
            WHEN insufficient_privilege THEN
                    RAISE NOTICE 'Skipping default table privileges to migrationUser';
        END;

        BEGIN
            EXECUTE format(
                'ALTER DEFAULT PRIVILEGES FOR ROLE "%s" IN SCHEMA "%s" GRANT USAGE, SELECT ON SEQUENCES TO "%s"',
                '{{migrationUser}}',
                '{{schemaName}}',
                '{{appUser}}'
            );
        EXCEPTION
            WHEN insufficient_privilege THEN
                    RAISE NOTICE 'Skipping default sequence privileges to migrationUser';
        END;
    END IF;
END
$$;
""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"DROP SCHEMA IF EXISTS ""{AppDbContext.SchemaName}"";");
        }
    }
}
