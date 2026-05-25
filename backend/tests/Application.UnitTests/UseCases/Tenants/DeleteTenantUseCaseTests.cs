using Application.Repositories;
using Application.Services;
using Application.UseCases.Tenants;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;
using Moq;
using Xunit;

namespace Application.UnitTests.UseCases.Tenants
{
    /// <summary>
    /// <see cref="DeleteTenantUseCase"/>のテスト
    /// </summary>
    public class DeleteTenantUseCaseTests
    {
        private readonly Mock<IAuthorizeService> _authorizeServiceMock;
        private readonly Mock<ITenantRepository> _tenantRepositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly DeleteTenantUseCase _useCase;

        public DeleteTenantUseCaseTests()
        {
            _authorizeServiceMock = new Mock<IAuthorizeService>();
            _tenantRepositoryMock = new Mock<ITenantRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _useCase = new DeleteTenantUseCase(_authorizeServiceMock.Object, _tenantRepositoryMock.Object, _uowMock.Object);
        }

        [Fact(DisplayName = "テナント削除に成功する")]
        public async Task ExecuteAsync_Should_Success()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var targetId = TenantId.New();

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.SystemAdmin)).Returns(true);
            _uowMock.Setup(x => x.CreateTenantIdScope(targetId.ToString())).Returns(Mock.Of<IDisposable>());
            _tenantRepositoryMock.Setup(x => x.DeleteAsync(targetId)).ReturnsAsync(1);

            var result = await _useCase.ExecuteAsync(tenantId, actorId, targetId);

            Assert.Equal(1, result);
            _tenantRepositoryMock.Verify(x => x.DeleteAsync(targetId), Times.Once);
        }

        [Fact(DisplayName = "テナント削除に失敗する (権限不足)")]
        public async Task ExecuteAsync_Should_Fail_When_Unauthorized()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var targetId = TenantId.New();

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.SystemAdmin)).Returns(false);

            await Assert.ThrowsAsync<AppForbiddenException>(() => _useCase.ExecuteAsync(tenantId, actorId, targetId));

            _tenantRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<TenantId>()), Times.Never);
        }

        [Fact(DisplayName = "テナント削除に失敗する (自分のテナントを削除)")]
        public async Task ExecuteAsync_Should_Fail_When_Delete_Own_Tenant()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.SystemAdmin)).Returns(true);

            await Assert.ThrowsAsync<AppValidateException>(() => _useCase.ExecuteAsync(tenantId, actorId, tenantId));

            _tenantRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<TenantId>()), Times.Never);
        }
    }
}
