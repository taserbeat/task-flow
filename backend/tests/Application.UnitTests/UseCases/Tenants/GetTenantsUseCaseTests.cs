using Application.Repositories;
using Application.Services;
using Application.UseCases.Tenants;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.UnitTests.UseCases.Tenants
{
    /// <summary>
    /// <see cref="GetTenantsUseCase"/>のテスト
    /// </summary>
    public class GetTenantsUseCaseTests
    {
        private readonly Mock<ILogger<GetTenantsUseCase>> _loggerMock;
        private readonly Mock<IAuthorizeService> _authorizeServiceMock;
        private readonly Mock<ITenantRepository> _tenantRepositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly GetTenantsUseCase _useCase;

        public GetTenantsUseCaseTests()
        {
            _loggerMock = new Mock<ILogger<GetTenantsUseCase>>();
            _authorizeServiceMock = new Mock<IAuthorizeService>();
            _tenantRepositoryMock = new Mock<ITenantRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _useCase = new GetTenantsUseCase(_loggerMock.Object, _authorizeServiceMock.Object, _tenantRepositoryMock.Object, _uowMock.Object);
        }

        [Fact(DisplayName = "テナント一覧取得に成功する")]
        public async Task ExecuteAsync_Should_Return_Tenants()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var now = DateTimeOffset.UtcNow;
            var tenants = new List<TenantEm>
            {
                TenantEm.Create(TenantId.New(), now, now, actorId, actorId, new TenantName("テナント1")),
                TenantEm.Create(TenantId.New(), now, now, actorId, actorId, new TenantName("テナント2"))
            };

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.SystemAdmin)).Returns(true);
            _uowMock.Setup(x => x.CreateBypassScope()).Returns(Mock.Of<IDisposable>());
            _tenantRepositoryMock.Setup(x => x.GetTenantsAsync()).ReturnsAsync(tenants);

            var result = await _useCase.ExecuteAsync(tenantId, actorId);

            Assert.Equal(2, result.Count());
            Assert.Contains(result, t => t.Name.Value == "テナント1");
            Assert.Contains(result, t => t.Name.Value == "テナント2");
        }

        [Fact(DisplayName = "テナント一覧取得に失敗する (権限不足)")]
        public async Task ExecuteAsync_Should_Fail_When_Unauthorized()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.SystemAdmin)).Returns(false);

            await Assert.ThrowsAsync<AppForbiddenException>(() => _useCase.ExecuteAsync(tenantId, actorId));

            _tenantRepositoryMock.Verify(x => x.GetTenantsAsync(), Times.Never);
        }
    }
}
