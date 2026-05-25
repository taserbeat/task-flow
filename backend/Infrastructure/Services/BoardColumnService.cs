using Application.Contexts;
using Application.Repositories;
using Application.Services;
using Domain.Entities.BoardColumns;
using Domain.Entities.TaskItems;
using Domain.Entities.Tenants;

namespace Infrastructure.Services
{
    /// <summary>
    /// ボード列のサービス
    /// </summary>
    public class BoardColumnService : IBoardColumnService
    {
        private readonly TimeProvider _timeProvider;
        private readonly IUserContext _userContext;
        private readonly IUnitOfWork _uow;
        private readonly ITaskItemRepository _taskItemRepository;

        public BoardColumnService(TimeProvider timeProvider, IUserContext userContext, IUnitOfWork uow, ITaskItemRepository taskItemRepository)
        {
            _timeProvider = timeProvider;
            _userContext = userContext;
            _uow = uow;
            _taskItemRepository = taskItemRepository;
        }

        public async Task RebalanceAsync(TenantId tenantId, BoardColumnId boardColumnId)
        {
            var taskItemEms = await _taskItemRepository.GetTaskItemsByBoardColumnAsync(tenantId, boardColumnId);

            var now = _timeProvider.GetUtcNow();

            var nextPosition = TaskItemPosition.NewInitPosition();
            foreach (var taskItemEm in taskItemEms)
            {
                taskItemEm.ChangePosition(nextPosition, now, _userContext.UserId);
                nextPosition = nextPosition.NewNextPosition();
            }

            await _uow.SaveChangesAsync();
        }
    }
}