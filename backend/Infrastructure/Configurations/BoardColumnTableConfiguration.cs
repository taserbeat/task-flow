using Domain.Entities.BoardColumns;
using Domain.Entities.Boards;
using Infrastructure.Extensions.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    /// <summary>
    /// ボード列テーブルの定義
    /// </summary>
    public class BoardColumnTableConfiguration : IEntityTypeConfiguration<BoardColumnEm>
    {
        public void Configure(EntityTypeBuilder<BoardColumnEm> builder)
        {
            // テーブル名
            builder.ToTable("board_columns", tableBuider =>
            {
                tableBuider.HasComment("ボード列テーブル");
            });

            // 主キー
            builder.HasKey(x => new
            {
                x.Id,
            });

            #region  カラム設定

            builder.ConfigureTenantAuditableColumns<BoardColumnEm, BoardColumnId>();

            // ======================================
            // ボードID (外部キー)
            // ======================================
            builder.Property(x => x.BoardId)
                .HasColumnName("board_id")
                .HasConversion(
                    v => v.Value,
                    v => (BoardId)Activator.CreateInstance(typeof(BoardId), v)!
                )
                .HasComment("ボードID")
                .IsRequired();

            // 外部キー制約 (BoardColumnEm -> BoardEm)
            // (Cascade: ボード削除時に該当ボードの列を削除)
            builder.HasOne(x => x.Board)
                .WithMany(x => x.Columns)
                .HasForeignKey(x => x.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            // ======================================
            // 列名
            // ======================================
            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(BoardColumnName.MaxLength)
                .HasConversion(
                    v => v.Value,
                    v => new BoardColumnName(v)
                )
                .HasComment("列名")
                .IsRequired();

            // ======================================
            // 位置
            // ======================================
            builder.Property(x => x.Position)
                .HasColumnName("position")
                .HasConversion(
                    v => v.Value,
                    v => new BoardColumnPosition(v)
                )
                .HasComment("位置")
                .IsRequired();

            // ユニーク制約
            // NOTE: (BoardId, Position) の組み合わせは一意とし、同一ボードで重複しないようにする
            builder.HasIndex(x => new
            {
                x.BoardId,
                x.Position,
            }).IsUnique();

            // インデックス
            builder.HasIndex(x => new { x.Position });

            #endregion
        }
    }
}