using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContexts
{
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// スキーマ名
        /// </summary>
        public static readonly string SchemaName = "tf";

        /// <summary>
        /// マイグレーション履歴テーブル名
        /// </summary>
        public static readonly string MigrationsHistoryTableName = "__EFMigrationsHistory";

        /// <summary>
        /// マイグレーション用のDBユーザー名
        /// </summary>
        public static readonly string MigrationUser = "tfAdmin";

        /// <summary>
        /// アプリケーション用のDBユーザー名
        /// </summary>
        public static readonly string AppUser = "tfApp";

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TenantEm> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // スキーマ名を設定
            modelBuilder.HasDefaultSchema(SchemaName);

            // テーブルの設定
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}