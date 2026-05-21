using Application.Repositories;
using Application.Services;
using Domain.Entities.BoardColumns;
using Domain.Entities.Boards;
using Domain.Entities.TaskItems;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;

namespace Application.UseCases.TaskItems
{
    /// <summary>
    /// タスクの更新ユースケース
    /// </summary>
    public class UpdateTaskItemUseCase
    {
        private readonly TimeProvider _timeProvider;
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly IUnitOfWork _uow;
        private readonly IBoardColumnService _boardColumnService;
        private readonly IExceptionService _exceptionService;
        private readonly IBoardColumnRepository _boardColumnRepository;

        public UpdateTaskItemUseCase(TimeProvider timeProvider, ITaskItemRepository taskItemRepository, IUnitOfWork uow, IBoardColumnService boardColumnService, IExceptionService exceptionService, IBoardColumnRepository boardColumnRepository)
        {
            _timeProvider = timeProvider;
            _taskItemRepository = taskItemRepository;
            _uow = uow;
            _boardColumnService = boardColumnService;
            _exceptionService = exceptionService;
            _boardColumnRepository = boardColumnRepository;
        }

        public async Task ExecuteAsync(TenantId tenantId, UserId actorId, BoardId targetBoardId, BoardColumnId targetColumnId, TaskItemId targetTaskId, UpdateTaskItemParam param)
        {
            // ボードと列の存在チェック
            var isTargetColumnExists = await _boardColumnRepository.ExistsByIdAsync(tenantId, targetBoardId, targetColumnId);
            if (!isTargetColumnExists)
            {
                throw new AppNotFoundException("指定のボードと列に指定のタスクは存在しません。");
            }

            // タスクと列の存在チェック
            var targetTaskItemEm = await _taskItemRepository.GetByIdAsync(tenantId, targetTaskId);
            if (targetTaskItemEm is null || targetTaskItemEm.BoardColumnId != targetColumnId)
            {
                throw new AppNotFoundException("指定のタスクは存在しません。");
            }

            var now = _timeProvider.GetUtcNow();

            // 担当者
            if (param.IsReleaseAssignee.HasValue && param.IsReleaseAssignee.Value)
            {
                // 担当者を削除
                targetTaskItemEm.ReleaseAssignee(now, actorId);
            }
            else if (param.AssigneeId.HasValue && targetTaskItemEm.AssigneeId?.Value != param.AssigneeId.Value)
            {
                // 担当者を変更・設定
                var newAssigneeId = UserId.New(param.AssigneeId.Value);
                targetTaskItemEm.Assign(newAssigneeId, now, actorId);
            }

            // タイトル
            if (!string.IsNullOrWhiteSpace(param.Title))
            {
                var newTitle = new TaskItemTitle(param.Title);
                targetTaskItemEm.ChangeTitle(newTitle, now, actorId);
            }

            // 説明
            if (!string.IsNullOrWhiteSpace(param.Description))
            {
                var newDescription = new TaskItemDescription(param.Description);
                targetTaskItemEm.ChangeDescription(newDescription, now, actorId);
            }

            // 優先度
            if (param.Priority is not null && targetTaskItemEm.Priority != param.Priority)
            {
                targetTaskItemEm.ChangePriority(param.Priority.Value, now, actorId);
            }

            // 期限日
            if (param.IsDeleteDueDate.HasValue && param.IsDeleteDueDate.Value)
            {
                // 期限日を削除
                targetTaskItemEm.DeleteDueDate(now, actorId);
            }
            else if (param.DueDate.HasValue && targetTaskItemEm.DueDate != param.DueDate.Value)
            {
                targetTaskItemEm.ChangeDueDate(param.DueDate.Value, now, actorId);
            }

            // NOTE:
            // 列の変更とタスクの位置変更は同時に行わないようにする

            TaskItemEm? prevTaskItemEm;
            TaskItemEm? nextTaskItemEm;
            var prevTaskItemId = TaskItemId.New(param.PreviousTaskItemId);
            var nextTaskItemId = TaskItemId.New(param.NextTaskItemId);

            if (param.BoardColumnId.HasValue && param.BoardColumnId.Value != targetTaskId.Value)
            {
                // 列の変更を行う場合、変更後の列の最後に追加する
                var newColumnId = BoardColumnId.New(param.BoardColumnId.Value);
                var lastPosition = await _taskItemRepository.GetLastPositionAsync(tenantId, newColumnId);
                var newPosition = lastPosition is null ? TaskItemPosition.NewInitPosition() : lastPosition.NewNextPosition();

                targetTaskItemEm.ChangeColumn(newColumnId, newPosition, now, actorId);
            }
            else if (param.PreviousTaskItemId != null || param.NextTaskItemId != null)
            {
                // 位置の変更を行う場合

                #region 位置変更の検証

                if (param.PreviousTaskItemId == param.NextTaskItemId)
                {
                    // prev == nextは位置変更できない
                    throw new AppValidateException("前後のタスクが同じため位置を変更できません。");
                }

                if (targetColumnId.Value == param.PreviousTaskItemId)
                {
                    // 自分自身を前後にあると指定することはできない
                    throw new AppValidateException("自身を前にあるタスクとして指定することはできません。");
                }

                if (targetColumnId.Value == param.NextTaskItemId)
                {
                    // 自分自身を前後にあると指定することはできない
                    throw new AppValidateException("自身を後ろにあるタスクとして指定することはできません。");
                }

                // 前後のタスクを取得
                prevTaskItemEm = param.PreviousTaskItemId is null ? null : await _taskItemRepository.GetByIdAsync(tenantId, prevTaskItemId);
                nextTaskItemEm = param.NextTaskItemId is null ? null : await _taskItemRepository.GetByIdAsync(tenantId, nextTaskItemId);

                // 既に採番が必要と判明している場合は採番する
                if (prevTaskItemEm is not null && nextTaskItemEm is not null)
                {
                    var middlePosition = TaskItemPosition.NewMiddlePosition(prevTaskItemEm.Position, nextTaskItemEm.Position);
                    if (prevTaskItemEm.Position == middlePosition || nextTaskItemEm.Position == middlePosition)
                    {
                        // 採番
                        await _boardColumnService.RebalanceAsync(tenantId, targetColumnId);

                        // 採番後のタスク情報を再取得
                        prevTaskItemEm = await _taskItemRepository.GetByIdAsync(tenantId, prevTaskItemId);
                        nextTaskItemEm = await _taskItemRepository.GetByIdAsync(tenantId, nextTaskItemId);
                    }
                }

                if (prevTaskItemEm is null && nextTaskItemEm is null)
                {
                    // 両方存在しないタスクの場合は位置変更できない
                    throw new AppValidateException("前後のタスクが不明のため位置を変更できません。");
                }

                if (prevTaskItemEm is not null && prevTaskItemEm.BoardColumnId != targetColumnId)
                {
                    // 同一の列でないなら位置変更できない
                    throw new AppValidateException("前のタスクが異なる列のため位置を変更できません。");
                }

                if (nextTaskItemEm is not null && nextTaskItemEm.BoardColumnId != targetColumnId)
                {
                    // 同一の列でないなら位置変更できない
                    throw new AppValidateException("後ろのタスクが異なる列のため位置を変更できません。");
                }

                // 銭湯位置に変更する場合、指定のタスクが銭湯であったのかチェック
                if (prevTaskItemEm is null && nextTaskItemEm is not null)
                {
                    var firstPosition = await _taskItemRepository.GetFirstPositionAsync(tenantId, nextTaskItemEm.BoardColumnId);
                    if (firstPosition is null || nextTaskItemEm.Position != firstPosition)
                    {
                        // 先頭位置ではないタスクなので位置変更できない
                        throw new AppValidateException("先頭位置ではないため位置を変更できません。");
                    }
                }

                // 2つのタスクの間に位置変更する場合、前後2つが連続であるかをチェック
                if (prevTaskItemEm is not null && nextTaskItemEm is not null)
                {
                    var rangeCount = await _taskItemRepository.CountPositionRangeAsync(tenantId, targetColumnId, prevTaskItemEm.Position, nextTaskItemEm.Position);
                    if (rangeCount != 2)
                    {
                        // 前後のタスクが非連続、または、順序が間違いなので位置変更できない
                        throw new AppValidateException("前後のタスクが連続していないため位置を変更できません。");
                    }
                }

                // 最終位置に変更する場合、指定のタスクが最終位置であったのかチェック
                if (prevTaskItemEm is not null && nextTaskItemEm is null)
                {
                    var lastPosition = await _taskItemRepository.GetLastPositionAsync(tenantId, targetColumnId);
                    if (lastPosition is null || prevTaskItemEm.Position != lastPosition)
                    {
                        // 最終位置ではないタスクなので位置変更できない
                        throw new AppValidateException("最終位置ではないため位置を変更できません。");
                    }
                }

                #endregion

                #region 位置の更新

                ChangePosition(targetTaskItemEm, now, actorId, prevTaskItemEm, nextTaskItemEm);

                #endregion
            }

            try
            {
                await _uow.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!_exceptionService.IsUniqueConstraintViolation(ex))
                {
                    // ユニーク制約以外の例外はそのままスロー
                    throw;
                }

                // 採番してリトライ
                await _boardColumnService.RebalanceAsync(tenantId, targetColumnId);

                prevTaskItemEm = param.PreviousTaskItemId is null ? null : await _taskItemRepository.GetByIdAsync(tenantId, prevTaskItemId);
                nextTaskItemEm = param.NextTaskItemId is null ? null : await _taskItemRepository.GetByIdAsync(tenantId, nextTaskItemId);

                ChangePosition(targetTaskItemEm, now, actorId, prevTaskItemEm, nextTaskItemEm);

                await _uow.SaveChangesAsync();
            }
        }

        private void ChangePosition(TaskItemEm target, DateTimeOffset now, UserId actorId, TaskItemEm? prev, TaskItemEm? next)
        {
            // 先頭の位置に変更
            if (prev is null && next is not null)
            {
                var newPosition = next.Position.NewPreviousPosition();
                target.ChangePosition(newPosition, now, actorId);
            }

            // 中間の位置に変更
            if (prev is not null && next is not null)
            {
                var newPosition = TaskItemPosition.NewMiddlePosition(prev.Position, next.Position);
                target.ChangePosition(newPosition, now, actorId);
            }

            // 末尾の位置に変更
            if (prev is not null && next is null)
            {
                var newPosition = prev.Position.NewNextPosition();
                target.ChangePosition(newPosition, now, actorId);
            }
        }
    }

    /// <summary>
    /// タスク更新ユースケースのパラメータ
    /// </summary>
    /// <value></value>
    public record UpdateTaskItemParam
    {
        /// <summary>
        /// 変更後の列ID
        /// </summary>
        /// <value></value>
        public Guid? BoardColumnId { get; init; }

        /// <summary>
        /// 変更後の担当者ID
        /// </summary>
        /// <value></value>
        public Guid? AssigneeId { get; init; }

        /// <summary>
        /// 担当者を解除するか?
        /// </summary>
        /// <value></value>
        public bool? IsReleaseAssignee { get; init; }

        /// <summary>
        /// タイトル
        /// </summary>
        /// <value></value>
        public string? Title { get; init; }

        /// <summary>
        /// 説明
        /// </summary>
        /// <value></value>
        public string? Description { get; init; }

        /// <summary>
        /// 優先度
        /// </summary>
        /// <value></value>
        public TaskItemPriorityEnum? Priority { get; init; }

        /// <summary>
        /// 変更後の期限日
        /// </summary>
        /// <value></value>
        public DateTimeOffset? DueDate { get; init; }

        /// <summary>
        /// 期限日を削除するか?
        /// </summary>
        /// <value></value>
        public bool? IsDeleteDueDate { get; init; }

        /// <summary>
        /// 変更後の位置の1つ前のタスクID (位置変更がある場合のみ)
        /// </summary>
        /// <value></value>
        public Guid? PreviousTaskItemId { get; init; }

        /// <summary>
        /// 変更後の位置の1つ後のタスクID (位置変更がある場合のみ)
        /// </summary>
        /// <value></value>
        public Guid? NextTaskItemId { get; init; }
    }
}