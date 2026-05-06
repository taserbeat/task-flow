using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateBaseTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tf");

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "tf",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "エンティティのID"),
                    name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, comment: "ロール名"),
                    level = table.Column<int>(type: "integer", nullable: false, comment: "ロールレベル")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                },
                comment: "ロールテーブル");

            migrationBuilder.CreateTable(
                name: "tenants",
                schema: "tf",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "エンティティのID"),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, comment: "テナント名"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "作成日時"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true, comment: "作成者"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "最終更新日時"),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true, comment: "最終更新者")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.id);
                },
                comment: "テナントテーブル");

            migrationBuilder.CreateTable(
                name: "users",
                schema: "tf",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "エンティティのID"),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, comment: "メールアドレス"),
                    password_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false, comment: "パスワードハッシュ"),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "ロールID"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, comment: "有効状態"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "作成日時"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true, comment: "作成者"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "最終更新日時"),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true, comment: "最終更新者"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "テナントID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "tf",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "ユーザーテーブル");

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                schema: "tf",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_name",
                schema: "tf",
                table: "tenants",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                schema: "tf",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email_is_active",
                schema: "tf",
                table: "users",
                columns: new[] { "email", "is_active" });

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                schema: "tf",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_tenant_id",
                schema: "tf",
                table: "users",
                column: "tenant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenants",
                schema: "tf");

            migrationBuilder.DropTable(
                name: "users",
                schema: "tf");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "tf");
        }
    }
}
