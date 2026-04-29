using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContexts
{
    /// <summary>
    /// TaskFlowのDbContext
    /// </summary>
    public class TaskFlowDbContext : DbContext
    {
        /// <summary>
        /// スキーマ名
        /// </summary>
        public static readonly string SchemaName = "tf";

        public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : base(options)
        {
        }

        public DbSet<TenantEm> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // スキーマ名を設定
            modelBuilder.HasDefaultSchema(SchemaName);

            // テーブルの設定
            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskFlowDbContext).Assembly);
            modelBuilder.ApplyConfiguration(new TenantTableConfiguration());
        }
    }
}