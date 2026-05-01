using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateTenantsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tf");

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

            migrationBuilder.CreateIndex(
                name: "IX_tenants_name",
                schema: "tf",
                table: "tenants",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenants",
                schema: "tf");
        }
    }
}
