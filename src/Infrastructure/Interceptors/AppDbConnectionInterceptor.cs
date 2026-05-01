using System.Data.Common;
using Application.Contexts;
using Infrastructure.Extensions.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Interceptors
{
    /// <summary>
    /// DBとのコネクション開始・終了前後に処理を挟むインターセプター
    /// </summary>
    public class AppDbConnectionInterceptor : DbConnectionInterceptor
    {
        private readonly ILogger<AppDbConnectionInterceptor> _logger;
        private readonly ITenantContext _tenantContext;

        public AppDbConnectionInterceptor(ILogger<AppDbConnectionInterceptor> logger, ITenantContext tenantContext)
        {
            _logger = logger;
            _tenantContext = tenantContext;
        }

        public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            using var command = connection.CreateCommand();

            if (_tenantContext.IsBypass)
            {
                // バイパスモードの場合、バイパスのセッション変数を有効にする
                command.CommandText = $@"
                    SET {MigrationBuilderExtensions.RlsBypassSessionVariable} = 'on';
                ";
            }
            else if (!string.IsNullOrWhiteSpace(_tenantContext.TenantId))
            {
                // テナントIDが設定されている場合、セッション変数にテナントIDを設定
                command.CommandText = $@"
                    SET {MigrationBuilderExtensions.RlsSessionVariable} = '{_tenantContext.TenantId}';
                    SET {MigrationBuilderExtensions.RlsBypassSessionVariable} = 'off';
                ";
            }
            else
            {
                // テナントIDが設定されていない場合、セキュリティを考慮してDB接続を拒否
                _logger.LogError("テナントIDが設定されていません。RLSが適切に機能しないため、DB接続を拒否しました。");

                throw new InvalidOperationException("テナントIDが設定されていません。");
            }

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}