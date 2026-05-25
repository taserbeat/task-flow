using Application.Repositories;
using Application.Services;
using Application.UseCases.Boards;
using Domain.Entities.Boards;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;
using Moq;
using Xunit;

namespace Application.UnitTests.UseCases.Boards
{
    /// <summary>
    /// <see cref="CreateBoardUseCase"/>のテスト
    /// </summary>
    public class CreateBoardUseCaseTests
    {
        private readonly Mock<IAuthorizeService> _authorizeServiceMock;
        private readonly Mock<TimeProvider> _timeProviderMock;
        private readonly Mock<IBoardRepository> _boardRepositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly CreateBoardUseCase _useCase;

        public CreateBoardUseCaseTests()
        {
            _authorizeServiceMock = new Mock<IAuthorizeService>();
            _timeProviderMock = new Mock<TimeProvider>();
            _boardRepositoryMock = new Mock<IBoardRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _useCase = new CreateBoardUseCase(_authorizeServiceMock.Object, _timeProviderMock.Object, _boardRepositoryMock.Object, _uowMock.Object);
        }

        [Fact(DisplayName = "ボード作成に成功する")]
        public async Task ExecuteAsync_Should_Success()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var param = new CreateBoardParam { Name = "テストボード" };
            var now = DateTimeOffset.UtcNow;

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(true);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _boardRepositoryMock.Setup(x => x.AddAsync(It.IsAny<BoardEm>())).Returns(Task.CompletedTask);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, param);

            _boardRepositoryMock.Verify(x => x.AddAsync(It.Is<BoardEm>(b => b.Name.Value == "テストボード")), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "ボード作成に失敗する (権限不足)")]
        public async Task ExecuteAsync_Should_Fail_When_Unauthorized()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var param = new CreateBoardParam { Name = "テストボード" };

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(false);

            await Assert.ThrowsAsync<AppForbiddenException>(() => _useCase.ExecuteAsync(tenantId, actorId, param));

            _boardRepositoryMock.Verify(x => x.AddAsync(It.IsAny<BoardEm>()), Times.Never);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }
    }
}