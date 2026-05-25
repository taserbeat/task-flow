using Application.Repositories;
using Application.Services;
using Application.UseCases.Boards;
using Domain.Entities.Boards;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Exceptions;
using Moq;
using Xunit;

namespace Application.UnitTests.UseCases.Boards
{
    /// <summary>
    /// <see cref="DeleteBoardUseCase"/>のテスト
    /// </summary>
    public class DeleteBoardUseCaseTests
    {
        private readonly Mock<IAuthorizeService> _authorizeServiceMock;
        private readonly Mock<IBoardRepository> _boardRepositoryMock;
        private readonly DeleteBoardUseCase _useCase;

        public DeleteBoardUseCaseTests()
        {
            _authorizeServiceMock = new Mock<IAuthorizeService>();
            _boardRepositoryMock = new Mock<IBoardRepository>();
            _useCase = new DeleteBoardUseCase(_authorizeServiceMock.Object, _boardRepositoryMock.Object);
        }

        [Fact(DisplayName = "ボード削除に成功する")]
        public async Task ExecuteAsync_Should_Success()
        {
            var tenantId = TenantId.New();
            var boardId = BoardId.New();

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(true);
            _boardRepositoryMock.Setup(x => x.DeleteAsync(tenantId, boardId)).ReturnsAsync(1);

            var result = await _useCase.ExecuteAsync(tenantId, boardId);

            Assert.Equal(1, result);
            _boardRepositoryMock.Verify(x => x.DeleteAsync(tenantId, boardId), Times.Once);
        }

        [Fact(DisplayName = "ボード削除に失敗する (権限不足)")]
        public async Task ExecuteAsync_Should_Fail_When_Unauthorized()
        {
            var tenantId = TenantId.New();
            var boardId = BoardId.New();

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(false);

            await Assert.ThrowsAsync<AppForbiddenException>(() => _useCase.ExecuteAsync(tenantId, boardId));

            _boardRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<TenantId>(), It.IsAny<BoardId>()), Times.Never);
        }
    }
}
