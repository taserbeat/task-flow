using Domain.Entities.Roles;
using Domain.Entities.Users;
using Infrastructure.Extensions.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    /// <summary>
    /// ユーザーテーブルの定義
    /// </summary>
    public class UserTableConfiguration : IEntityTypeConfiguration<UserEm>
    {
        public void Configure(EntityTypeBuilder<UserEm> builder)
        {
            // テーブル名
            builder.ToTable("users", tableBuider =>
            {
                tableBuider.HasComment("ユーザーテーブル");
            });

            // 主キー
            builder.HasKey(x => new
            {
                x.Id,
            });

            #region  カラム設定

            builder.ConfigureTenantAuditableColumns<UserEm, UserId>();

            // 外部キー制約 (UserEm -> TenantEm)
            // (Cascade: テナント削除時に該当テナントに所属するユーザーを削除)
            builder.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            // ======================================
            // メールアドレス
            // ======================================
            builder.Property(x => x.Email)
                .HasColumnName("email")
                .HasMaxLength(UserEmail.MaxLength)
                .HasConversion(
                    v => v.Value,
                    v => new UserEmail(v)
                )
                .HasComment("メールアドレス")
                .IsRequired();

            // ユニーク制約
            // NOTE:
            // メールアドレスはすべてのテナントで一意とし、ログインの際にテナントを特定する。
            // これにより、ユーザーは複数のテナントに所属できなくなるが、テナント選択のUIを省略可能となる。
            builder.HasIndex(x => new { x.Email }).IsUnique();

            // ======================================
            // パスワードハッシュ
            // ======================================
            builder.Property(x => x.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(512)
                .HasConversion(
                    v => v.Value,
                    v => new UserPasswordHash(v)
                )
                .HasComment("パスワードハッシュ")
                .IsRequired();

            // ======================================
            // ロールID (外部キー)
            // ======================================
            builder.Property(x => x.RoleId)
                .HasColumnName("role_id")
                .HasConversion(
                    v => v.Value,
                    v => (RoleId)Activator.CreateInstance(typeof(RoleId), v)!
                )
                .HasComment("ロールID")
                .IsRequired();

            // 外部キー制約
            builder.HasIndex(x => new { x.RoleId });

            builder.HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);  // ロール削除防止

            // ======================================
            // 氏名
            // ======================================
            builder.OwnsOne(
                x => x.Username,
                username =>
                {
                    // ValueObject(姓+名)を設定
                    username.Property(x => x.LastName)
                        .HasColumnName("last_name")
                        .HasMaxLength(UserName.MaxLastNameLength)
                        .IsRequired();

                    username.Property(x => x.FirstName)
                        .HasColumnName("first_name")
                        .HasMaxLength(UserName.MaxFirstNameLength)
                        .IsRequired();
                }
            );

            // ======================================
            // 有効状態
            // ======================================
            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .HasComment("有効状態")
                .IsRequired();

            // インデックス (認証処理高速化)
            builder.HasIndex(x => new { x.Email, x.IsActive });

            #endregion
        }
    }
}