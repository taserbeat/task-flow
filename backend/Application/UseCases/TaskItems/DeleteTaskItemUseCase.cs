using Application.Repositories;
using Domain.Entities.TaskItems;
using Domain.Entities.Tenants;

namespace Application.UseCases.TaskItems
{
    /// <summary>
    /// タスクの削除ユースケース
    /// </summary>
    public class DeleteTaskItemUseCase
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public DeleteTaskItemUseCase(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task ExecuteAsync(TenantId tenantId, TaskItemId taskItemId)
        {
            await _taskItemRepository.DeleteAsync(tenantId, taskItemId);
        }
    }
}