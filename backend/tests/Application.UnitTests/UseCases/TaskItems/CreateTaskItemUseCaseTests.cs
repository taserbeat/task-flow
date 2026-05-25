using Application.Repositories;
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
    /// <see cref="CreateTaskItemUseCase"/>のテスト
    /// </summary>
    public class CreateTaskItemUseCaseTests
    {
        private readonly Mock<TimeProvider> _timeProviderMock;
        private readonly Mock<IBoardColumnRepository> _boardColumnRepositoryMock;
        private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly CreateTaskItemUseCase _useCase;

        public CreateTaskItemUseCaseTests()
        {
            _timeProviderMock = new Mock<TimeProvider>();
            _boardColumnRepositoryMock = new Mock<IBoardColumnRepository>();
            _taskItemRepositoryMock = new Mock<ITaskItemRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _useCase = new CreateTaskItemUseCase(_timeProviderMock.Object, _boardColumnRepositoryMock.Object, _taskItemRepositoryMock.Object, _uowMock.Object, _userRepositoryMock.Object);
        }

        [Fact(DisplayName = "タスク作成に成功する (担当者なし)")]
        public async Task ExecuteAsync_Should_Success_Without_Assignee()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var now = DateTimeOffset.UtcNow;
            var param = new CreateTaskItemParam
            {
                BoardColumnId = columnId.Value,
                AssigneeId = null,
                Title = "タスク1",
                Description = "説明",
                Priority = TaskItemPriorityEnum.Medium,
                DueDate = null
            };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetLastPositionAsync(tenantId, columnId)).ReturnsAsync((TaskItemPosition?)null);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _taskItemRepositoryMock.Setup(x => x.AddAsync(It.IsAny<TaskItemEm>())).Returns(Task.CompletedTask);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, param);

            _taskItemRepositoryMock.Verify(x => x.AddAsync(It.Is<TaskItemEm>(t => t.Title.Value == "タスク1" && t.AssigneeId == null)), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "タスク作成に成功する (担当者あり)")]
        public async Task ExecuteAsync_Should_Success_With_Assignee()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var assigneeId = UserId.New();
            var now = DateTimeOffset.UtcNow;
            var param = new CreateTaskItemParam
            {
                BoardColumnId = columnId.Value,
                AssigneeId = assigneeId.Value,
                Title = "タスク1",
                Description = "説明",
                Priority = TaskItemPriorityEnum.High,
                DueDate = now.AddDays(7)
            };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _userRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, assigneeId)).ReturnsAsync(true);
            _taskItemRepositoryMock.Setup(x => x.GetLastPositionAsync(tenantId, columnId)).ReturnsAsync((TaskItemPosition?)null);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _taskItemRepositoryMock.Setup(x => x.AddAsync(It.IsAny<TaskItemEm>())).Returns(Task.CompletedTask);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, param);

            _taskItemRepositoryMock.Verify(x => x.AddAsync(It.Is<TaskItemEm>(t => t.Title.Value == "タスク1" && t.AssigneeId == assigneeId)), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "タスク作成に失敗する (列が存在しない)")]
        public async Task ExecuteAsync_Should_Fail_When_Column_Not_Found()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var param = new CreateTaskItemParam
            {
                BoardColumnId = columnId.Value,
                AssigneeId = null,
                Title = "タスク1",
                Description = "説明",
                Priority = TaskItemPriorityEnum.Medium,
                DueDate = null
            };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(false);

            await Assert.ThrowsAsync<AppNotFoundException>(() => _useCase.ExecuteAsync(tenantId, actorId, boardId, param));

            _taskItemRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TaskItemEm>()), Times.Never);
        }

        [Fact(DisplayName = "タスク作成に失敗する (担当者が存在しない)")]
        public async Task ExecuteAsync_Should_Fail_When_Assignee_Not_Found()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var assigneeId = UserId.New();
            var param = new CreateTaskItemParam
            {
                BoardColumnId = columnId.Value,
                AssigneeId = assigneeId.Value,
                Title = "タスク1",
                Description = "説明",
                Priority = TaskItemPriorityEnum.Medium,
                DueDate = null
            };

            _boardColumnRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId, columnId)).ReturnsAsync(true);
            _userRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, assigneeId)).ReturnsAsync(false);

            await Assert.ThrowsAsync<AppNotFoundException>(() => _useCase.ExecuteAsync(tenantId, actorId, boardId, param));

            _taskItemRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TaskItemEm>()), Times.Never);
        }
    }
}
