using Application.Repositories;
using Application.Services;
using Application.UseCases.Boards;
using Domain.Entities.Boards;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;
using Moq;
using Xunit;

namespace Application.UnitTests.UseCases.Boards
{
    /// <summary>
    /// <see cref="UpdateBoardUseCase"/>のテスト
    /// </summary>
    public class UpdateBoardUseCaseTests
    {
        private readonly Mock<IAuthorizeService> _authorizeServiceMock;
        private readonly Mock<TimeProvider> _timeProviderMock;
        private readonly Mock<IBoardRepository> _boardRepositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly UpdateBoardUseCase _useCase;

        public UpdateBoardUseCaseTests()
        {
            _authorizeServiceMock = new Mock<IAuthorizeService>();
            _timeProviderMock = new Mock<TimeProvider>();
            _boardRepositoryMock = new Mock<IBoardRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _useCase = new UpdateBoardUseCase(_authorizeServiceMock.Object, _timeProviderMock.Object, _boardRepositoryMock.Object, _uowMock.Object);
        }

        [Fact(DisplayName = "ボード更新に成功する")]
        public async Task ExecuteAsync_Should_Success()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var now = DateTimeOffset.UtcNow;
            var boardEm = BoardEm.Create(boardId, tenantId, new BoardName("旧ボード名"), now, now, actorId, actorId);
            var param = new UpdateBoardParam { Name = "新ボード名" };

            _boardRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, boardId)).ReturnsAsync(boardEm);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, boardId, param);

            Assert.Equal("新ボード名", boardEm.Name.Value);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "ボード更新に失敗する (ボードが存在しない)")]
        public async Task ExecuteAsync_Should_Fail_When_Board_Not_Found()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var boardId = BoardId.New();
            var param = new UpdateBoardParam { Name = "新ボード名" };

            _boardRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, boardId)).ReturnsAsync((BoardEm?)null);

            await Assert.ThrowsAsync<AppNotFoundException>(() => _useCase.ExecuteAsync(tenantId, actorId, boardId, param));
        }
    }
}
