using Application.Repositories;
using Application.Services;
using Application.UseCases.BoardColumns;
using Domain.Entities.BoardColumns;
using Domain.Entities.Boards;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;
using Moq;
using Xunit;

namespace Application.UnitTests.UseCases.BoardColumns
{
    /// <summary>
    /// <see cref="UpdateBoardColumnUseCase"/>のテスト
    /// </summary>
    public class UpdateBoardColumnUseCaseTests
    {
        private readonly Mock<TimeProvider> _timeProviderMock;
        private readonly Mock<IBoardColumnRepository> _boardColumnRepositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IBoardService> _boardServiceMock;
        private readonly Mock<IExceptionService> _exceptionServiceMock;
        private readonly UpdateBoardColumnUseCase _useCase;

        public UpdateBoardColumnUseCaseTests()
        {
            _timeProviderMock = new Mock<TimeProvider>();
            _boardColumnRepositoryMock = new Mock<IBoardColumnRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _boardServiceMock = new Mock<IBoardService>();
            _exceptionServiceMock = new Mock<IExceptionService>();
            _useCase = new UpdateBoardColumnUseCase(_timeProviderMock.Object, _boardColumnRepositoryMock.Object, _uowMock.Object, _boardServiceMock.Object, _exceptionServiceMock.Object);
        }

        [Fact(DisplayName = "列名の更新に成功する")]
        public async Task ExecuteAsync_Should_Success_When_Update_Name()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var now = DateTimeOffset.UtcNow;
            var columnEm = BoardColumnEm.Create(columnId, tenantId, boardId, new BoardColumnName("ToDo"), new BoardColumnPosition(100), now, now, actorId, actorId);
            var param = new UpdateBoardColumnParam { Name = "In Progress" };

            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, columnId)).ReturnsAsync(columnEm);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, param);

            Assert.Equal("In Progress", columnEm.Name.Value);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "列の更新に失敗する (列が存在しない)")]
        public async Task ExecuteAsync_Should_Fail_When_Column_Not_Found()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var param = new UpdateBoardColumnParam { Name = "In Progress" };

            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, columnId)).ReturnsAsync((BoardColumnEm?)null);

            await Assert.ThrowsAsync<AppNotFoundException>(() => _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, param));
        }

        [Fact(DisplayName = "列の更新に失敗する (異なるボードの列)")]
        public async Task ExecuteAsync_Should_Fail_When_Column_Belongs_To_Different_Board()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var differentBoardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var now = DateTimeOffset.UtcNow;
            var columnEm = BoardColumnEm.Create(columnId, tenantId, differentBoardId, new BoardColumnName("ToDo"), new BoardColumnPosition(100), now, now, actorId, actorId);
            var param = new UpdateBoardColumnParam { Name = "In Progress" };

            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, columnId)).ReturnsAsync(columnEm);

            await Assert.ThrowsAsync<AppNotFoundException>(() => _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, param));
        }

        [Fact(DisplayName = "位置を先頭に変更する")]
        public async Task ExecuteAsync_Should_Success_When_Move_To_First()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var nextColumnId = BoardColumnId.New();
            var now = DateTimeOffset.UtcNow;
            var columnEm = BoardColumnEm.Create(columnId, tenantId, boardId, new BoardColumnName("Done"), new BoardColumnPosition(300), now, now, actorId, actorId);
            var nextColumnEm = BoardColumnEm.Create(nextColumnId, tenantId, boardId, new BoardColumnName("ToDo"), new BoardColumnPosition(100), now, now, actorId, actorId);
            var param = new UpdateBoardColumnParam { PreviousColumnId = null, NextColumnId = nextColumnId.Value };

            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, columnId)).ReturnsAsync(columnEm);
            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, nextColumnId)).ReturnsAsync(nextColumnEm);
            _boardColumnRepositoryMock.Setup(x => x.GetFirstPositionAsync(tenantId, boardId)).ReturnsAsync(new BoardColumnPosition(100));
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, param);

            Assert.Equal(50, columnEm.Position.Value);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "位置を最後に変更する")]
        public async Task ExecuteAsync_Should_Success_When_Move_To_Last()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var prevColumnId = BoardColumnId.New();
            var now = DateTimeOffset.UtcNow;
            var columnEm = BoardColumnEm.Create(columnId, tenantId, boardId, new BoardColumnName("ToDo"), new BoardColumnPosition(100), now, now, actorId, actorId);
            var prevColumnEm = BoardColumnEm.Create(prevColumnId, tenantId, boardId, new BoardColumnName("Done"), new BoardColumnPosition(300), now, now, actorId, actorId);
            var param = new UpdateBoardColumnParam { PreviousColumnId = prevColumnId.Value, NextColumnId = null };

            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, columnId)).ReturnsAsync(columnEm);
            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, prevColumnId)).ReturnsAsync(prevColumnEm);
            _boardColumnRepositoryMock.Setup(x => x.GetLastPositionAsync(tenantId, boardId)).ReturnsAsync(new BoardColumnPosition(300));
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, param);

            Assert.Equal(400, columnEm.Position.Value);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "位置を中間に変更する")]
        public async Task ExecuteAsync_Should_Success_When_Move_To_Middle()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var prevColumnId = BoardColumnId.New();
            var nextColumnId = BoardColumnId.New();
            var now = DateTimeOffset.UtcNow;
            var columnEm = BoardColumnEm.Create(columnId, tenantId, boardId, new BoardColumnName("Done"), new BoardColumnPosition(300), now, now, actorId, actorId);
            var prevColumnEm = BoardColumnEm.Create(prevColumnId, tenantId, boardId, new BoardColumnName("ToDo"), new BoardColumnPosition(100), now, now, actorId, actorId);
            var nextColumnEm = BoardColumnEm.Create(nextColumnId, tenantId, boardId, new BoardColumnName("In Progress"), new BoardColumnPosition(200), now, now, actorId, actorId);
            var param = new UpdateBoardColumnParam { PreviousColumnId = prevColumnId.Value, NextColumnId = nextColumnId.Value };

            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, columnId)).ReturnsAsync(columnEm);
            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, prevColumnId)).ReturnsAsync(prevColumnEm);
            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, nextColumnId)).ReturnsAsync(nextColumnEm);
            _boardColumnRepositoryMock.Setup(x => x.CountPositionRangeAsync(tenantId, boardId, prevColumnEm.Position, nextColumnEm.Position)).ReturnsAsync(2);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, param);

            Assert.Equal(150, columnEm.Position.Value);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "位置変更に失敗する (前後の列が同じ)")]
        public async Task ExecuteAsync_Should_Fail_When_Previous_And_Next_Are_Same()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var sameColumnId = BoardColumnId.New();
            var now = DateTimeOffset.UtcNow;
            var columnEm = BoardColumnEm.Create(columnId, tenantId, boardId, new BoardColumnName("ToDo"), new BoardColumnPosition(100), now, now, actorId, actorId);
            var param = new UpdateBoardColumnParam { PreviousColumnId = sameColumnId.Value, NextColumnId = sameColumnId.Value };

            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, columnId)).ReturnsAsync(columnEm);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);

            await Assert.ThrowsAsync<AppValidateException>(() => _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, param));
        }

        [Fact(DisplayName = "位置変更に失敗する (自分自身を前の列として指定)")]
        public async Task ExecuteAsync_Should_Fail_When_Previous_Is_Self()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var nextColumnId = BoardColumnId.New();
            var now = DateTimeOffset.UtcNow;
            var columnEm = BoardColumnEm.Create(columnId, tenantId, boardId, new BoardColumnName("ToDo"), new BoardColumnPosition(100), now, now, actorId, actorId);
            var param = new UpdateBoardColumnParam { PreviousColumnId = columnId.Value, NextColumnId = nextColumnId.Value };

            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, columnId)).ReturnsAsync(columnEm);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);

            await Assert.ThrowsAsync<AppValidateException>(() => _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, param));
        }

        [Fact(DisplayName = "位置変更に失敗する (前後の列が両方存在しない)")]
        public async Task ExecuteAsync_Should_Fail_When_Both_Previous_And_Next_Not_Found()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var prevColumnId = BoardColumnId.New();
            var nextColumnId = BoardColumnId.New();
            var now = DateTimeOffset.UtcNow;
            var columnEm = BoardColumnEm.Create(columnId, tenantId, boardId, new BoardColumnName("ToDo"), new BoardColumnPosition(100), now, now, actorId, actorId);
            var param = new UpdateBoardColumnParam { PreviousColumnId = prevColumnId.Value, NextColumnId = nextColumnId.Value };

            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, columnId)).ReturnsAsync(columnEm);
            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, prevColumnId)).ReturnsAsync((BoardColumnEm?)null);
            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, nextColumnId)).ReturnsAsync((BoardColumnEm?)null);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);

            await Assert.ThrowsAsync<AppValidateException>(() => _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, param));
        }

        [Fact(DisplayName = "位置変更に失敗する (前後の列が非連続)")]
        public async Task ExecuteAsync_Should_Fail_When_Previous_And_Next_Are_Not_Consecutive()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var columnId = BoardColumnId.New();
            var prevColumnId = BoardColumnId.New();
            var nextColumnId = BoardColumnId.New();
            var now = DateTimeOffset.UtcNow;
            var columnEm = BoardColumnEm.Create(columnId, tenantId, boardId, new BoardColumnName("Done"), new BoardColumnPosition(400), now, now, actorId, actorId);
            var prevColumnEm = BoardColumnEm.Create(prevColumnId, tenantId, boardId, new BoardColumnName("ToDo"), new BoardColumnPosition(100), now, now, actorId, actorId);
            var nextColumnEm = BoardColumnEm.Create(nextColumnId, tenantId, boardId, new BoardColumnName("Review"), new BoardColumnPosition(300), now, now, actorId, actorId);
            var param = new UpdateBoardColumnParam { PreviousColumnId = prevColumnId.Value, NextColumnId = nextColumnId.Value };

            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, columnId)).ReturnsAsync(columnEm);
            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, prevColumnId)).ReturnsAsync(prevColumnEm);
            _boardColumnRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, nextColumnId)).ReturnsAsync(nextColumnEm);
            _boardColumnRepositoryMock.Setup(x => x.CountPositionRangeAsync(tenantId, boardId, prevColumnEm.Position, nextColumnEm.Position)).ReturnsAsync(3);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);

            await Assert.ThrowsAsync<AppValidateException>(() => _useCase.ExecuteAsync(tenantId, actorId, boardId, columnId, param));
        }
    }
}
