using Application.Repositories;
using Application.UseCases.Boards;
using Domain.Entities.Boards;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Moq;
using Xunit;

namespace Application.UnitTests.UseCases.Boards
{
    /// <summary>
    /// <see cref="GetBoardUseCase"/>のテスト
    /// </summary>
    public class GetBoardUseCaseTests
    {
        private readonly Mock<IBoardRepository> _boardRepositoryMock;
        private readonly GetBoardUseCase _useCase;

        public GetBoardUseCaseTests()
        {
            _boardRepositoryMock = new Mock<IBoardRepository>();
            _useCase = new GetBoardUseCase(_boardRepositoryMock.Object);
        }

        [Fact(DisplayName = "ボード取得に成功する")]
        public async Task ExecuteAsync_Should_Return_Board()
        {
            var tenantId = TenantId.New();
            var boardId = BoardId.New();
            var userId = UserId.New();
            var now = DateTimeOffset.UtcNow;
            var boardEm = BoardEm.Create(boardId, tenantId, new BoardName("テストボード"), now, now, userId, userId);

            _boardRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, boardId)).ReturnsAsync(boardEm);

            var result = await _useCase.ExecuteAsync(tenantId, boardId);

            Assert.NotNull(result);
            Assert.Equal(boardId, result.Id);
            Assert.Equal("テストボード", result.Name.Value);
        }

        [Fact(DisplayName = "ボード取得に失敗する (ボードが存在しない)")]
        public async Task ExecuteAsync_Should_Return_Null_When_Board_Not_Found()
        {
            var tenantId = TenantId.New();
            var boardId = BoardId.New();

            _boardRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, boardId)).ReturnsAsync((BoardEm?)null);

            var result = await _useCase.ExecuteAsync(tenantId, boardId);

            Assert.Null(result);
        }
    }
}
