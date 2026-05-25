using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateTaskTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "boards",
                schema: "tf",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "エンティティのID"),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, comment: "ボード名"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "作成日時"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true, comment: "作成者"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "最終更新日時"),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true, comment: "最終更新者"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "テナントID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boards", x => x.id);
                    table.ForeignKey(
                        name: "FK_boards_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "tf",
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "ボードテーブル");

            migrationBuilder.CreateTable(
                name: "board_columns",
                schema: "tf",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "エンティティのID"),
                    board_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "ボードID"),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, comment: "列名"),
                    position = table.Column<int>(type: "integer", nullable: false, comment: "位置"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "作成日時"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true, comment: "作成者"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "最終更新日時"),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true, comment: "最終更新者"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "テナントID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_board_columns", x => x.id);
                    table.ForeignKey(
                        name: "FK_board_columns_boards_board_id",
                        column: x => x.board_id,
                        principalSchema: "tf",
                        principalTable: "boards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "ボード列テーブル");

            migrationBuilder.CreateTable(
                name: "tasks",
                schema: "tf",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "エンティティのID"),
                    board_column_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "ボード列ID"),
                    assignee_id = table.Column<Guid>(type: "uuid", nullable: true, comment: "担当者ID"),
                    title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, comment: "タイトル"),
                    description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false, comment: "説明"),
                    priority = table.Column<int>(type: "integer", nullable: false, comment: "優先度"),
                    due_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "期限日"),
                    position = table.Column<int>(type: "integer", nullable: false, comment: "位置"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "作成日時"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true, comment: "作成者"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "最終更新日時"),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true, comment: "最終更新者"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "テナントID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.id);
                    table.ForeignKey(
                        name: "FK_tasks_board_columns_board_column_id",
                        column: x => x.board_column_id,
                        principalSchema: "tf",
                        principalTable: "board_columns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tasks_users_assignee_id",
                        column: x => x.assignee_id,
                        principalSchema: "tf",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_tasks_users_created_by",
                        column: x => x.created_by,
                        principalSchema: "tf",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                },
                comment: "タスクテーブル");

            migrationBuilder.CreateIndex(
                name: "IX_board_columns_board_id_position",
                schema: "tf",
                table: "board_columns",
                columns: new[] { "board_id", "position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_board_columns_position",
                schema: "tf",
                table: "board_columns",
                column: "position");

            migrationBuilder.CreateIndex(
                name: "IX_board_columns_tenant_id",
                schema: "tf",
                table: "board_columns",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_boards_tenant_id",
                schema: "tf",
                table: "boards",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_assignee_id",
                schema: "tf",
                table: "tasks",
                column: "assignee_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_board_column_id_position",
                schema: "tf",
                table: "tasks",
                columns: new[] { "board_column_id", "position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tasks_created_by",
                schema: "tf",
                table: "tasks",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_due_date",
                schema: "tf",
                table: "tasks",
                column: "due_date");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_position",
                schema: "tf",
                table: "tasks",
                column: "position");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_tenant_id",
                schema: "tf",
                table: "tasks",
                column: "tenant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tasks",
                schema: "tf");

            migrationBuilder.DropTable(
                name: "board_columns",
                schema: "tf");

            migrationBuilder.DropTable(
                name: "boards",
                schema: "tf");
        }
    }
}
