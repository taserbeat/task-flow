using Domain.Entities.Tenants;
using Infrastructure.Extensions.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    /// <summary>
    /// テナントテーブルの定義
    /// </summary>
    public class TenantTableConfiguration : IEntityTypeConfiguration<TenantEm>
    {
        public void Configure(EntityTypeBuilder<TenantEm> builder)
        {
            // テーブル名
            builder.ToTable("tenants", tableBuider =>
            {
                tableBuider.HasComment("テナントテーブル");
            });

            // 主キー
            builder.HasKey(x => new
            {
                x.Id,
            });

            // 制約

            // インデックス
            builder.HasIndex(x => new
            {
                x.Name,
            });

            #region カラム設定

            builder.ConfigureAuditableColumns<TenantEm, TenantId>();

            // テナント名
            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(128)
                .HasComment("テナント名")
                .IsRequired();

            #endregion
        }
    }
}