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
    /// <see cref="GetBoardsUseCase"/>のテスト
    /// </summary>
    public class GetBoardsUseCaseTests
    {
        private readonly Mock<IBoardRepository> _boardRepositoryMock;
        private readonly GetBoardsUseCase _useCase;

        public GetBoardsUseCaseTests()
        {
            _boardRepositoryMock = new Mock<IBoardRepository>();
            _useCase = new GetBoardsUseCase(_boardRepositoryMock.Object);
        }

        [Fact(DisplayName = "ボード一覧取得に成功する")]
        public async Task ExecuteAsync_Should_Return_Boards()
        {
            var tenantId = TenantId.New();
            var userId = UserId.New();
            var now = DateTimeOffset.UtcNow;

            var boards = new List<BoardEm>
            {
                BoardEm.Create(BoardId.New(), tenantId, new BoardName("ボード1"), now,now, userId, userId),
                BoardEm.Create(BoardId.New(), tenantId, new BoardName("ボード2"), now,now, userId, userId)
            };

            _boardRepositoryMock.Setup(x => x.GetBoardsAsync(tenantId)).ReturnsAsync(boards);

            var result = await _useCase.ExecuteAsync(tenantId);

            Assert.Equal(2, result.Count());
            Assert.Contains(result, b => b.Name.Value == "ボード1");
            Assert.Contains(result, b => b.Name.Value == "ボード2");
        }

        [Fact(DisplayName = "ボード一覧取得に成功する (空のリスト)")]
        public async Task ExecuteAsync_Should_Return_Empty_List()
        {
            var tenantId = TenantId.New();

            _boardRepositoryMock.Setup(x => x.GetBoardsAsync(tenantId)).ReturnsAsync(new List<BoardEm>());

            var result = await _useCase.ExecuteAsync(tenantId);

            Assert.Empty(result);
        }
    }
}
