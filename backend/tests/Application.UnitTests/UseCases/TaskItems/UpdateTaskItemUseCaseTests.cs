using Application.Repositories;
using Application.Services;
using Application.UseCases.TaskItems;
using Domain.Entities.BoardColumns;
using Domain.Entities.Boards;
using Domain.Entities.TaskItems;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;
using Moq;
using Xunit;

namespace Application.UnitTests.UseCases.TaskItems
{
    /// <summary>
    /// <see cref="UpdateTaskItemUseCase"/>のテスト
    /// </summary>
    public class UpdateTaskItemUseCaseTests
    {
        private readonly Mock<TimeProvider> _timeProviderMock;
        private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IBoardColumnService> _boardColumnServiceMock;
        private readonly Mock<IExceptionService> _exceptionServiceMock;
        private readonly Mock<IBoardColumnRepository> _boardColumnRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UpdateTaskItemUseCase _useCase;

        public UpdateTaskItemUseCaseTests()
        {
            _timeProviderMock = new Mock<TimeProvider>();
            _taskItemRepositoryMock = new Mock<ITaskItemRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _boardColumnServiceMock = new Mock<IBoardColumnService>();
            _exceptionServiceMock = new Mock<IExceptionService>();
            _boardColumnRepositoryMock = new Mock<IBoardColumnRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _useCase = new UpdateTaskItemUseCase(_timeProviderMock.Object, _taskItemRepositoryMock.Object, _uowMock.Object, _boardColumnServiceMock.Object, _exceptionServiceMock.Object, _boardColumnRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact(DisplayName = "タイトルの更新に成功する")]
        public async Task ExecuteAsync_Should_Success_When_Update_Title()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var taskId = TaskItemId.New();
            var now = DateTimeOffset.UtcNow;
            var taskEm = TaskItemEm.Create(taskId, tenantId, columnId, null, new TaskItemTitle("旧タイトル"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, null, new TaskItemPosition(100), now, now, actorId, actorId);
            var param = new UpdateTaskItemParam { Title = "新タイトル" };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, taskId)).ReturnsAsync(taskEm);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, taskId, param);

            Assert.Equal("新タイトル", taskEm.Title.Value);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "担当者の設定に成功する")]
        public async Task ExecuteAsync_Should_Success_When_Assign()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var taskId = TaskItemId.New();
            var assigneeId = UserId.New();
            var now = DateTimeOffset.UtcNow;
            var taskEm = TaskItemEm.Create(taskId, tenantId, columnId, null, new TaskItemTitle("タスク1"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, null, new TaskItemPosition(100), now, now, actorId, actorId);
            var param = new UpdateTaskItemParam { AssigneeId = assigneeId.Value };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, taskId)).ReturnsAsync(taskEm);
            _userRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, assigneeId)).ReturnsAsync(true);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, taskId, param);

            Assert.Equal(assigneeId, taskEm.AssigneeId);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "担当者の解除に成功する")]
        public async Task ExecuteAsync_Should_Success_When_Release_Assignee()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var taskId = TaskItemId.New();
            var assigneeId = UserId.New();
            var now = DateTimeOffset.UtcNow;
            var taskEm = TaskItemEm.Create(taskId, tenantId, columnId, assigneeId, new TaskItemTitle("タスク1"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, null, new TaskItemPosition(100), now, now, actorId, actorId);
            var param = new UpdateTaskItemParam { IsReleaseAssignee = true };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, taskId)).ReturnsAsync(taskEm);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, taskId, param);

            Assert.Null(taskEm.AssigneeId);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "優先度の更新に成功する")]
        public async Task ExecuteAsync_Should_Success_When_Update_Priority()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var taskId = TaskItemId.New();
            var now = DateTimeOffset.UtcNow;
            var taskEm = TaskItemEm.Create(taskId, tenantId, columnId, null, new TaskItemTitle("タスク1"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Low, null, new TaskItemPosition(100), now, now, actorId, actorId);
            var param = new UpdateTaskItemParam { Priority = TaskItemPriorityEnum.High };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, taskId)).ReturnsAsync(taskEm);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, taskId, param);

            Assert.Equal(TaskItemPriorityEnum.High, taskEm.Priority);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "期限日の設定に成功する")]
        public async Task ExecuteAsync_Should_Success_When_Set_DueDate()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var taskId = TaskItemId.New();
            var now = DateTimeOffset.UtcNow;
            var dueDate = now.AddDays(7);
            var taskEm = TaskItemEm.Create(taskId, tenantId, columnId, null, new TaskItemTitle("タスク1"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, null, new TaskItemPosition(100), now, now, actorId, actorId);
            var param = new UpdateTaskItemParam { DueDate = dueDate };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, taskId)).ReturnsAsync(taskEm);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, taskId, param);

            Assert.Equal(dueDate, taskEm.DueDate);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "期限日の削除に成功する")]
        public async Task ExecuteAsync_Should_Success_When_Delete_DueDate()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var taskId = TaskItemId.New();
            var now = DateTimeOffset.UtcNow;
            var taskEm = TaskItemEm.Create(taskId, tenantId, columnId, null, new TaskItemTitle("タスク1"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, now.AddDays(7), new TaskItemPosition(100), now, now, actorId, actorId);
            var param = new UpdateTaskItemParam { IsDeleteDueDate = true };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, taskId)).ReturnsAsync(taskEm);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, taskId, param);

            Assert.Null(taskEm.DueDate);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "列の変更に成功する")]
        public async Task ExecuteAsync_Should_Success_When_Change_Column()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var oldColumnId = BoardColumnId.New();
            var newColumnId = BoardColumnId.New();
            var taskId = TaskItemId.New();
            var now = DateTimeOffset.UtcNow;
            var taskEm = TaskItemEm.Create(taskId, tenantId, oldColumnId, null, new TaskItemTitle("タスク1"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, null, new TaskItemPosition(100), now, now, actorId, actorId);
            var param = new UpdateTaskItemParam { BoardColumnId = newColumnId.Value };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, oldColumnId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, taskId)).ReturnsAsync(taskEm);
            _taskItemRepositoryMock.Setup(x => x.GetLastPositionAsync(tenantId, newColumnId)).ReturnsAsync(new TaskItemPosition(200));
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, oldColumnId, taskId, param);

            Assert.Equal(newColumnId, taskEm.BoardColumnId);
            Assert.Equal(300, taskEm.Position.Value);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "タスクの更新に失敗する (列が存在しない)")]
        public async Task ExecuteAsync_Should_Fail_When_Column_Not_Found()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var taskId = TaskItemId.New();
            var param = new UpdateTaskItemParam { Title = "新タイトル" };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(false);

            await Assert.ThrowsAsync<AppNotFoundException>(() => _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, taskId, param));
        }

        [Fact(DisplayName = "タスクの更新に失敗する (タスクが存在しない)")]
        public async Task ExecuteAsync_Should_Fail_When_Task_Not_Found()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var taskId = TaskItemId.New();
            var param = new UpdateTaskItemParam { Title = "新タイトル" };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, taskId)).ReturnsAsync((TaskItemEm?)null);

            await Assert.ThrowsAsync<AppNotFoundException>(() => _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, taskId, param));
        }

        [Fact(DisplayName = "タスクの更新に失敗する (担当者が存在しない)")]
        public async Task ExecuteAsync_Should_Fail_When_Assignee_Not_Found()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var taskId = TaskItemId.New();
            var assigneeId = UserId.New();
            var now = DateTimeOffset.UtcNow;
            var taskEm = TaskItemEm.Create(taskId, tenantId, columnId, null, new TaskItemTitle("タスク1"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, null, new TaskItemPosition(100), now, now, actorId, actorId);
            var param = new UpdateTaskItemParam { AssigneeId = assigneeId.Value };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, taskId)).ReturnsAsync(taskEm);
            _userRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, assigneeId)).ReturnsAsync(false);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);

            await Assert.ThrowsAsync<AppNotFoundException>(() => _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, taskId, param));
        }

        [Fact(DisplayName = "位置を先頭に変更する")]
        public async Task ExecuteAsync_Should_Success_When_Move_To_First()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var taskId = TaskItemId.New();
            var nextTaskId = TaskItemId.New();
            var now = DateTimeOffset.UtcNow;
            var taskEm = TaskItemEm.Create(taskId, tenantId, columnId, null, new TaskItemTitle("タスク3"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, null, new TaskItemPosition(300), now, now, actorId, actorId);
            var nextTaskEm = TaskItemEm.Create(nextTaskId, tenantId, columnId, null, new TaskItemTitle("タスク1"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, null, new TaskItemPosition(100), now, now, actorId, actorId);
            var param = new UpdateTaskItemParam { PreviousTaskItemId = null, NextTaskItemId = nextTaskId.Value };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, taskId)).ReturnsAsync(taskEm);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, nextTaskId)).ReturnsAsync(nextTaskEm);
            _taskItemRepositoryMock.Setup(x => x.GetFirstPositionAsync(tenantId, columnId)).ReturnsAsync(new TaskItemPosition(100));
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, taskId, param);

            Assert.Equal(50, taskEm.Position.Value);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "位置を最後に変更する")]
        public async Task ExecuteAsync_Should_Success_When_Move_To_Last()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var taskId = TaskItemId.New();
            var prevTaskId = TaskItemId.New();
            var now = DateTimeOffset.UtcNow;
            var taskEm = TaskItemEm.Create(taskId, tenantId, columnId, null, new TaskItemTitle("タスク1"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, null, new TaskItemPosition(100), now, now, actorId, actorId);
            var prevTaskEm = TaskItemEm.Create(prevTaskId, tenantId, columnId, null, new TaskItemTitle("タスク3"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, null, new TaskItemPosition(300), now, now, actorId, actorId);
            var param = new UpdateTaskItemParam { PreviousTaskItemId = prevTaskId.Value, NextTaskItemId = null };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, taskId)).ReturnsAsync(taskEm);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, prevTaskId)).ReturnsAsync(prevTaskEm);
            _taskItemRepositoryMock.Setup(x => x.GetLastPositionAsync(tenantId, columnId)).ReturnsAsync(new TaskItemPosition(300));
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, taskId, param);

            Assert.Equal(400, taskEm.Position.Value);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "位置を中間に変更する")]
        public async Task ExecuteAsync_Should_Success_When_Move_To_Middle()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var taskId = TaskItemId.New();
            var prevTaskId = TaskItemId.New();
            var nextTaskId = TaskItemId.New();
            var now = DateTimeOffset.UtcNow;
            var taskEm = TaskItemEm.Create(taskId, tenantId, columnId, null, new TaskItemTitle("タスク3"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, null, new TaskItemPosition(300), now, now, actorId, actorId);
            var prevTaskEm = TaskItemEm.Create(prevTaskId, tenantId, columnId, null, new TaskItemTitle("タスク1"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, null, new TaskItemPosition(100), now, now, actorId, actorId);
            var nextTaskEm = TaskItemEm.Create(nextTaskId, tenantId, columnId, null, new TaskItemTitle("タスク2"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, null, new TaskItemPosition(200), now, now, actorId, actorId);
            var param = new UpdateTaskItemParam { PreviousTaskItemId = prevTaskId.Value, NextTaskItemId = nextTaskId.Value };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, taskId)).ReturnsAsync(taskEm);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, prevTaskId)).ReturnsAsync(prevTaskEm);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, nextTaskId)).ReturnsAsync(nextTaskEm);
            _taskItemRepositoryMock.Setup(x => x.CountPositionRangeAsync(tenantId, columnId, prevTaskEm.Position, nextTaskEm.Position)).ReturnsAsync(2);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, taskId, param);

            Assert.Equal(150, taskEm.Position.Value);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "位置変更に失敗する (前後のタスクが同じ)")]
        public async Task ExecuteAsync_Should_Fail_When_Previous_And_Next_Are_Same()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var taskId = TaskItemId.New();
            var sameTaskId = TaskItemId.New();
            var now = DateTimeOffset.UtcNow;
            var taskEm = TaskItemEm.Create(taskId, tenantId, columnId, null, new TaskItemTitle("タスク1"), new TaskItemDescription("説明"), TaskItemPriorityEnum.Medium, null, new TaskItemPosition(100), now, now, actorId, actorId);
            var param = new UpdateTaskItemParam { PreviousTaskItemId = sameTaskId.Value, NextTaskItemId = sameTaskId.Value };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, taskId)).ReturnsAsync(taskEm);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);

            await Assert.ThrowsAsync<AppValidateException>(() => _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, taskId, param));
        }
    }
}
