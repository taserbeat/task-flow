using Application.Repositories;
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
    /// <see cref="CreateBoardColumnUseCase"/>のテスト
    /// </summary>
    public class CreateBoardColumnUseCaseTests
    {
        private readonly Mock<TimeProvider> _timeProviderMock;
        private readonly Mock<IBoardRepository> _boardRepositoryMock;
        private readonly Mock<IBoardColumnRepository> _boardColumnRepositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly CreateBoardColumnUseCase _useCase;

        public CreateBoardColumnUseCaseTests()
        {
            _timeProviderMock = new Mock<TimeProvider>();
            _boardRepositoryMock = new Mock<IBoardRepository>();
            _boardColumnRepositoryMock = new Mock<IBoardColumnRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _useCase = new CreateBoardColumnUseCase(_timeProviderMock.Object, _boardRepositoryMock.Object, _boardColumnRepositoryMock.Object, _uowMock.Object);
        }

        [Fact(DisplayName = "ボード列作成に成功する (初回)")]
        public async Task ExecuteAsync_Should_Success_When_First_Column()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var now = DateTimeOffset.UtcNow;
            var param = new CreateBoardColumnParam { BoardId = boardId.Value, Name = "ToDo" };

            _boardRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId)).ReturnsAsync(true);
            _boardColumnRepositoryMock.Setup(x => x.GetLastPositionAsync(tenantId, boardId)).ReturnsAsync((BoardColumnPosition?)null);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _boardColumnRepositoryMock.Setup(x => x.AddAsync(It.IsAny<BoardColumnEm>())).Returns(Task.CompletedTask);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, param);

            _boardColumnRepositoryMock.Verify(x => x.AddAsync(It.Is<BoardColumnEm>(c => c.Name.Value == "ToDo" && c.Position.Value == 100)), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "ボード列作成に成功する (2番目以降)")]
        public async Task ExecuteAsync_Should_Success_When_Not_First_Column()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var now = DateTimeOffset.UtcNow;
            var lastPosition = new BoardColumnPosition(100);
            var param = new CreateBoardColumnParam { BoardId = boardId.Value, Name = "In Progress" };

            _boardRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId)).ReturnsAsync(true);
            _boardColumnRepositoryMock.Setup(x => x.GetLastPositionAsync(tenantId, boardId)).ReturnsAsync(lastPosition);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _boardColumnRepositoryMock.Setup(x => x.AddAsync(It.IsAny<BoardColumnEm>())).Returns(Task.CompletedTask);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, param);

            _boardColumnRepositoryMock.Verify(x => x.AddAsync(It.Is<BoardColumnEm>(c => c.Name.Value == "In Progress" && c.Position.Value == 200)), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "ボード列作成に失敗する (ボードが存在しない)")]
        public async Task ExecuteAsync_Should_Fail_When_Board_Not_Found()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var param = new CreateBoardColumnParam { BoardId = boardId.Value, Name = "ToDo" };

            _boardRepositoryMock.Setup(x => x.ExistsByIdAsync(tenantId, boardId)).ReturnsAsync(false);

            await Assert.ThrowsAsync<AppNotFoundException>(() => _useCase.ExecuteAsync(tenantId, actorId, param));

            _boardColumnRepositoryMock.Verify(x => x.AddAsync(It.IsAny<BoardColumnEm>()), Times.Never);
        }
    }
}
