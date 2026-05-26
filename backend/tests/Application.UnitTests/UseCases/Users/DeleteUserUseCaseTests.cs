using Application.Repositories;
using Application.Services;
using Application.UseCases.Users;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;
using Moq;
using Xunit;

namespace Application.UnitTests.UseCases.Users
{
    /// <summary>
    /// <see cref="DeleteUserUseCase"/>のテスト
    /// </summary>
    public class DeleteUserUseCaseTests
    {
        private readonly Mock<IAuthorizeService> _authorizeServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleService> _roleServiceMock;
        private readonly DeleteUserUseCase _useCase;

        public DeleteUserUseCaseTests()
        {
            _authorizeServiceMock = new Mock<IAuthorizeService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleServiceMock = new Mock<IRoleService>();
            _useCase = new DeleteUserUseCase(_authorizeServiceMock.Object, _userRepositoryMock.Object, _roleServiceMock.Object);
        }

        [Fact(DisplayName = "ユーザー削除に成功する")]
        public async Task ExecuteAsync_Should_Success()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var targetId = UserId.New();

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(true);
            _roleServiceMock.Setup(x => x.CanDeleteUserAsync(tenantId, actorId, targetId)).ReturnsAsync(true);
            _userRepositoryMock.Setup(x => x.DeleteAsync(tenantId, targetId)).ReturnsAsync(1);

            var result = await _useCase.ExecuteAsync(tenantId, actorId, targetId);

            Assert.Equal(1, result);
            _userRepositoryMock.Verify(x => x.DeleteAsync(tenantId, targetId), Times.Once);
        }

        [Fact(DisplayName = "ユーザー削除に失敗する (権限不足)")]
        public async Task ExecuteAsync_Should_Fail_When_Unauthorized()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var targetId = UserId.New();

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(false);

            await Assert.ThrowsAsync<AppForbiddenException>(() => _useCase.ExecuteAsync(tenantId, actorId, targetId));

            _userRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<TenantId>(), It.IsAny<UserId>()), Times.Never);
        }

        [Fact(DisplayName = "ユーザー削除に失敗する (自分自身を削除)")]
        public async Task ExecuteAsync_Should_Fail_When_Delete_Self()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(true);

            await Assert.ThrowsAsync<AppValidateException>(() => _useCase.ExecuteAsync(tenantId, actorId, actorId));

            _userRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<TenantId>(), It.IsAny<UserId>()), Times.Never);
        }

        [Fact(DisplayName = "ユーザー削除に失敗する (ロールレベルが不足)")]
        public async Task ExecuteAsync_Should_Fail_When_Role_Level_Insufficient()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var targetId = UserId.New();

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(true);
            _roleServiceMock.Setup(x => x.CanDeleteUserAsync(tenantId, actorId, targetId)).ReturnsAsync(false);

            await Assert.ThrowsAsync<AppForbiddenException>(() => _useCase.ExecuteAsync(tenantId, actorId, targetId));

            _userRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<TenantId>(), It.IsAny<UserId>()), Times.Never);
        }
    }
}
