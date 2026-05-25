using Domain.Entities.BoardColumns;
using Domain.Entities.TaskItems;
using Domain.Entities.Users;
using Infrastructure.Extensions.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    /// <summary>
    /// タスクテーブルの定義
    /// </summary>
    public class TaskItemTableConfiguration : IEntityTypeConfiguration<TaskItemEm>
    {
        public void Configure(EntityTypeBuilder<TaskItemEm> builder)
        {
            // テーブル名
            builder.ToTable("tasks", tableBuider =>
            {
                tableBuider.HasComment("タスクテーブル");
            });

            // 主キー
            builder.HasKey(x => new
            {
                x.Id,
            });

            #region カラム設定

            builder.ConfigureTenantAuditableColumns<TaskItemEm, TaskItemId>();

            // ======================================
            // 作成者 (外部キー)
            // ======================================
            builder.HasOne(x => x.Creator)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // ======================================
            // ボード列ID (外部キー)
            // ======================================
            builder.Property(x => x.BoardColumnId)
                .HasColumnName("board_column_id")
                .HasConversion(
                    v => v.Value,
                    v => (BoardColumnId)Activator.CreateInstance(typeof(BoardColumnId), v)!
                )
                .HasComment("ボード列ID")
                .IsRequired();

            // 外部キー制約 (TaskItemEm -> BoardColumnEm)
            // (Cascade: ボード列削除時に該当ボード列のタスクを削除)
            builder.HasOne(x => x.Column)
                .WithMany(x => x.TaskItems)
                .HasForeignKey(x => x.BoardColumnId)
                .OnDelete(DeleteBehavior.Cascade);

            // ======================================
            // 担当者ID
            // ======================================
            builder.Property(x => x.AssigneeId)
                .HasColumnName("assignee_id")
                .HasConversion(
                    v => v.HasValue ? v.Value.Value : (Guid?)null,
                    v => v.HasValue ? new UserId(v.Value) : null
                )
                .HasComment("担当者ID");

            // インデックス
            builder.HasIndex(x => new { x.AssigneeId });

            // 外部キー制約
            builder.HasOne(x => x.Assignee)
                .WithMany(x => x.AssignedTasks)
                .HasForeignKey(x => x.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull);  // 担当者の削除時にnullを設定

            // ======================================
            // タイトル
            // ======================================
            builder.Property(x => x.Title)
                .HasColumnName("title")
                .HasMaxLength(TaskItemTitle.MaxLength)
                .HasConversion(
                    v => v.Value,
                    v => new TaskItemTitle(v)
                )
                .HasComment("タイトル")
                .IsRequired();

            // ======================================
            // 説明
            // ======================================
            builder.Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(TaskItemDescription.MaxLength)
                .HasConversion(
                    v => v.Value,
                    v => new TaskItemDescription(v)
                )
                .HasComment("説明")
                .IsRequired();

            // ======================================
            // 優先度
            // ======================================
            builder.Property(x => x.Priority)
                .HasColumnName("priority")
                .HasConversion<int>()
                .HasComment("優先度")
                .IsRequired();

            // ======================================
            // 期限日
            // ======================================
            builder.Property(x => x.DueDate)
                .HasColumnName("due_date")
                .HasComment("期限日");

            // インデックス
            builder.HasIndex(x => new { x.DueDate });

            // ======================================
            // 位置
            // ======================================
            builder.Property(x => x.Position)
                .HasColumnName("position")
                .HasConversion(
                    v => v.Value,
                    v => new TaskItemPosition(v)
                )
                .HasComment("位置")
                .IsRequired();

            // ユニーク制約
            // NOTE: (BoardColumnId, Position) の組み合わせは一意とし、同一ボード列で重複しないようにする
            builder.HasIndex(x => new
            {
                x.BoardColumnId,
                x.Position,
            }).IsUnique();

            // インデックス
            builder.HasIndex(x => new { x.Position });

            #endregion
        }
    }
}