using Domain.Entities.Common;
using Domain.Entities.Common.ValueObjects;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
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
        /// <typeparam name="TId"></typeparam>
        public static void ConfigureBaseColumns<TEm, TId>(this EntityTypeBuilder<TEm> self)
        where TEm : BaseEm<TId>
        where TId : struct, IStronglyTypedId<Guid>
        {
            // ID
            self.Property(x => x.Id)
                .HasColumnName("id")
                .HasConversion(
                    v => v.Value,
                    v => (TId)Activator.CreateInstance(typeof(TId), v)!)
                .HasComment("エンティティのID")
                .IsRequired();
        }

        /// <summary>
        /// 監査可能なエンティティモデルのカラムを設定する
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TEm"></typeparam>
        /// <typeparam name="TId"></typeparam>
        public static void ConfigureAuditableColumns<TEm, TId>(this EntityTypeBuilder<TEm> self)
        where TEm : BaseAuditableEm<TId>
        where TId : struct, IStronglyTypedId<Guid>
        {
            // 基底のカラムを設定
            self.ConfigureBaseColumns<TEm, TId>();

            // 作成日時
            self.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasComment("作成日時")
                .IsRequired();

            // 作成者
            self.Property(x => x.CreatedBy)
                .HasColumnName("created_by")
                .HasConversion(
                    v => v.HasValue ? v.Value.Value : (Guid?)null,
                    v => v.HasValue ? new UserId(v.Value) : null
                )
                .HasComment("作成者");

            // 更新日時
            self.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasComment("最終更新日時")
                .IsRequired();

            // 更新者
            self.Property(x => x.UpdatedBy)
                .HasColumnName("updated_by")
                .HasConversion(
                    v => v.HasValue ? v.Value.Value : (Guid?)null,
                    v => v.HasValue ? new UserId(v.Value) : null
                )
                .HasComment("最終更新者");
        }

        /// <summary>
        /// テナントに属する監査可能なエンティティモデルのカラムを設定する
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TEm"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <returns></returns>
        public static void ConfigureTenantAuditableColumns<TEm, TId>(this EntityTypeBuilder<TEm> self)
        where TEm : BaseTenantAuditableEm<TId>
        where TId : struct, IStronglyTypedId<Guid>
        {
            // 監査可能なカラムを設定
            self.ConfigureAuditableColumns<TEm, TId>();

            // インデックス
            self.HasIndex(x => new { x.TenantId });

            // テナントID
            self.Property(x => x.TenantId)
                .HasColumnName("tenant_id")
                .HasConversion(
                    v => v.Value,
                    v => new TenantId(v)
                )
                .HasComment("テナントID")
                .IsRequired();
        }
    }
}