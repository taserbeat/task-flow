using Domain.Entities.Boards;
using Infrastructure.Extensions.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    /// <summary>
    /// ボードテーブルの定義
    /// </summary>
    public class BoardTableConfiguration : IEntityTypeConfiguration<BoardEm>
    {
        public void Configure(EntityTypeBuilder<BoardEm> builder)
        {
            // テーブル名
            builder.ToTable("boards", tableBuider =>
            {
                tableBuider.HasComment("ボードテーブル");
            });

            // 主キー
            builder.HasKey(x => new
            {
                x.Id,
            });

            #region カラム設定

            builder.ConfigureTenantAuditableColumns<BoardEm, BoardId>();

            // 外部キー制約 (BoardEm -> TenantEm)
            // (Cascade: テナント削除時に該当テナントのボードを削除)
            builder.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            // ======================================
            // ボード名
            // ======================================
            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(128)
                .HasConversion(
                    v => v.Value,
                    v => new BoardName(v)
                )
                .HasComment("ボード名")
                .IsRequired();

            #endregion
        }
    }
}