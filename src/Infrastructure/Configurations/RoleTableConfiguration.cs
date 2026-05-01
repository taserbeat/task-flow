using Domain.Entities.Roles;
using Infrastructure.Extensions.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Configurations
{
    /// <summary>
    /// ロールテーブルの定義
    /// </summary>
    public class RoleTableConfiguration : IEntityTypeConfiguration<RoleEm>
    {
        public void Configure(EntityTypeBuilder<RoleEm> builder)
        {
            // テーブル名
            builder.ToTable("roles", tableBuider =>
            {
                tableBuider.HasComment("ロールテーブル");
            });

            // 主キー
            builder.HasKey(x => new
            {
                x.Id,
            });

            // 制約
            builder.HasIndex(x => new { x.Name }).IsUnique();

            #region カラム設定

            builder.ConfigureBaseColumns<RoleEm, RoleId>();

            // ロール名
            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(32)
                .HasConversion(new EnumToStringConverter<RoleNameEnum>())
                .HasComment("ロール名")
                .IsRequired();

            #endregion
        }
    }
}