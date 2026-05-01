using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Extensions.EntityFrameworkCore
{
    /// <summary>
    /// <see cref="EntityTypeBuilder"/> の拡張メソッド
    /// </summary>
    public static class EntityTypeBuilderExtensions
    {
        /// <summary>
        /// 基底のエンティティモデルのカラムを設定する
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TEm"></typeparam>
        public static void ConfigureBaseColumns<TEm>(this EntityTypeBuilder<TEm> self) where TEm : BaseEm
        {
            // ID
            self.Property(x => x.Id)
                .HasColumnName("id")
                .HasComment("エンティティのID")
                .IsRequired();
        }

        /// <summary>
        /// 監査可能なエンティティモデルのカラムを設定する
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TEm"></typeparam>
        public static void ConfigureAuditableColumns<TEm>(this EntityTypeBuilder<TEm> self) where TEm : BaseAuditableEm
        {
            // 基底のカラムを設定
            self.ConfigureBaseColumns();

            // 作成日時
            self.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasComment("作成日時")
                .IsRequired();

            // 作成者
            self.Property(x => x.CreatedBy)
                .HasColumnName("created_by")
                .HasComment("作成者");

            // 更新日時
            self.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasComment("最終更新日時")
                .IsRequired();

            // 更新者
            self.Property(x => x.UpdatedBy)
                .HasColumnName("updated_by")
                .HasComment("最終更新者");
        }
    }
}