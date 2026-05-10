using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Extensions.EntityFrameworkCore
{
    /// <summary>
    /// <see cref="MigrationBuilder"/> の拡張メソッド
    /// </summary>
    public static class MigrationBuilderExtensions
    {
        /// <summary>
        /// RLSの対象とするカラム名
        /// </summary>
        public const string RlsTargetColumn = "tenant_id";

        /// <summary>
        /// RLSのセッション変数
        /// </summary>
        public const string RlsSessionVariable = "app.tenant_id";

        /// <summary>
        /// RLSのポリシー名
        /// </summary>
        public const string RlsPolicyName = "rls_tenant_policy";

        /// <summary>
        /// RLSのバイパスで使用するセッション変数
        /// </summary>
        public const string RlsBypassSessionVariable = "app.rls_bypass";

        /// <summary>
        /// RLSのバイパスポリシー名
        /// </summary>
        public const string RlsBypassPolicyName = "rls_bypass_policy";

        /// <summary>
        /// 指定テーブルのRow Level Securityを有効にする
        /// </summary>
        /// <param name="self"></param>
        /// <param name="schema">スキーマ名</param>
        /// <param name="tableName">テーブル名</param>
        public static void EnableRowLevelSecurity(this MigrationBuilder self, string schema, string tableName)
        {
            // RLSを有効化
            self.Sql($@"
                ALTER TABLE {schema}.{tableName} ENABLE ROW LEVEL SECURITY;
            ");

            // 分離ポリシーを作成
            self.Sql($@"
                CREATE POLICY {RlsPolicyName} ON {schema}.{tableName}
                USING ({RlsTargetColumn} = current_setting('{RlsSessionVariable}', true)::uuid);
            ");
        }

        /// <summary>
        /// 指定テーブルのRow Level Securityを無効にする
        /// </summary>
        /// <param name="self"></param>
        /// <param name="schema">スキーマ名</param>
        /// <param name="tableName">テーブル名</param>
        public static void DisableRowLevelSecurity(this MigrationBuilder self, string schema, string tableName)
        {
            // ポリシーを削除
            self.Sql($@"
                DROP POLICY IF EXISTS {RlsPolicyName} ON {schema}.{tableName};
            ");

            // RLSを無効化
            self.Sql($@"
                ALTER TABLE {schema}.{tableName} DISABLE ROW LEVEL SECURITY;
            ");
        }

        /// <summary>
        /// 指定テーブルのRow Level Security(バイパス設定)を有効にする
        /// </summary>
        /// <param name="self"></param>
        /// <param name="schema">スキーマ名</param>
        /// <param name="tableName">テーブル名</param>
        public static void EnableBypassRowLevelSecurity(this MigrationBuilder self, string schema, string tableName)
        {
            // バイパスポリシーを作成
            self.Sql($@"
                CREATE POLICY {RlsBypassPolicyName} ON {schema}.{tableName}
                TO PUBLIC
                USING (current_setting('{RlsBypassSessionVariable}', true) = 'on');
            ");
        }

        /// <summary>
        /// 指定テーブルのRow Level Security(バイパス設定)を無効にする
        /// </summary>
        /// <param name="self"></param>
        /// <param name="schema">スキーマ名</param>
        /// <param name="tableName">テーブル名</param>
        public static void DisableBypassRowLevelSecurity(this MigrationBuilder self, string schema, string tableName)
        {
            // バイパスポリシーを削除
            self.Sql($@"
                DROP POLICY IF EXISTS {RlsBypassPolicyName} ON {schema}.{tableName};
            ");
        }
    }
}