using Application.Repositories;
using Domain.Entities.BoardColumns;
using Domain.Entities.TaskItems;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;

namespace Application.UseCases.TaskItems
{
    /// <summary>
    /// タスクの作成ユースケース
    /// </summary>
    public class CreateTaskItemUseCase
    {
        private readonly TimeProvider _timeProvider;
        private readonly IBoardColumnRepository _boardColumnRepository;
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly IUnitOfWork _uow;

        public CreateTaskItemUseCase(TimeProvider timeProvider, IBoardColumnRepository boardColumnRepository, ITaskItemRepository taskItemRepository, IUnitOfWork uow)
        {
            _timeProvider = timeProvider;
            _boardColumnRepository = boardColumnRepository;
            _taskItemRepository = taskItemRepository;
            _uow = uow;
        }

        public async Task ExecuteAsync(TenantId tenantId, UserId actorId, CreateTaskItemParam param)
        {
            // ボード列の存在チェック
            var columnId = BoardColumnId.New(param.BoardColumnId);
            var columnEm = await _boardColumnRepository.GetByIdAsync(tenantId, columnId);
            if (columnEm is null)
            {
                throw new AppNotFoundException("指定の列は存在しません。");
            }

            // 列の最後に追加するので、最後の位置を取得
            var lastPosition = await _taskItemRepository.GetLastPositionAsync(tenantId, columnId);

            // 追加する位置を取得
            var newPosition = lastPosition is null ? TaskItemPosition.NewInitPosition() : lastPosition.NewNextPosition();

            var now = _timeProvider.GetUtcNow();

            // パラメータ作成
            var taskItemEm = TaskItemEm.Create(
                id: TaskItemId.New(),
                tenantId: tenantId,
                boardColumnId: BoardColumnId.New(param.BoardColumnId),
                assigneeId: param.AssigneeId is null ? null : UserId.New(param.AssigneeId.Value),
                title: new(param.Title),
                description: new(param.Description),
                priority: param.Priority,
                dueDate: param.DueDate,
                position: newPosition,
                createdAt: now,
                updatedAt: now,
                createdBy: actorId,
                updatedBy: actorId
            );

            // 登録
            await _taskItemRepository.AddAsync(taskItemEm);
            await _uow.SaveChangesAsync();
        }
    }

    /// <summary>
    /// タスク作成ユースケースのパラメータ
    /// </summary>
    /// <value></value>
    public record CreateTaskItemParam
    {
        /// <summary>
        /// 列ID
        /// </summary>
        /// <value></value>
        public required Guid BoardColumnId { get; set; }

        /// <summary>
        /// 担当者ID
        /// </summary>
        /// <value></value>
        public Guid? AssigneeId { get; set; }

        /// <summary>
        /// タイトル
        /// </summary>
        /// <value></value>
        public required string Title { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        /// <value></value>
        public required string Description { get; set; }

        /// <summary>
        /// 優先度
        /// </summary>
        /// <value></value>
        public required TaskItemPriorityEnum Priority { get; set; }

        /// <summary>
        /// 期限日
        /// </summary>
        /// <value></value>
        public DateTimeOffset? DueDate { get; set; }
    }
}