using Application.Repositories;
using Application.UseCases.Users;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Moq;
using Xunit;

namespace Application.UnitTests.UseCases.Users
{
    /// <summary>
    /// <see cref="GetCurrentUserUseCase"/>のテスト
    /// </summary>
    public class GetCurrentUserUseCaseTests
    {
        private readonly Mock<ITenantRepository> _tenantRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetCurrentUserUseCase _useCase;

        public GetCurrentUserUseCaseTests()
        {
            _tenantRepositoryMock = new Mock<ITenantRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _useCase = new GetCurrentUserUseCase(_tenantRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact(DisplayName = "現在のユーザー情報取得に成功する")]
        public async Task ExecuteAsync_Should_Return_Current_User()
        {
            var tenantId = TenantId.New();
            var userId = UserId.New();
            var roleId = RoleId.New();
            var now = DateTimeOffset.UtcNow;
            var tenantEm = TenantEm.Create(tenantId, now, now, userId, userId, new TenantName("テナント1"));
            var userEm = UserEm.Create(userId, tenantId, now, now, userId, userId, new UserEmail("test@example.com"), new UserPasswordHash("hash"), new UserName("田中", "太郎"), roleId);

            _tenantRepositoryMock.Setup(x => x.GetByIdAsync(tenantId)).ReturnsAsync(tenantEm);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, userId, true)).ReturnsAsync(userEm);

            var (tenant, user) = await _useCase.ExecuteAsync(tenantId, userId);

            Assert.NotNull(tenant);
            Assert.NotNull(user);
            Assert.Equal(tenantId, tenant.Id);
            Assert.Equal(userId, user.Id);
        }

        [Fact(DisplayName = "現在のユーザー情報取得に成功する (テナントが存在しない)")]
        public async Task ExecuteAsync_Should_Return_Null_Tenant_When_Not_Found()
        {
            var tenantId = TenantId.New();
            var userId = UserId.New();
            var roleId = RoleId.New();
            var now = DateTimeOffset.UtcNow;
            var userEm = UserEm.Create(userId, tenantId, now, now, userId, userId, new UserEmail("test@example.com"), new UserPasswordHash("hash"), new UserName("田中", "太郎"), roleId);

            _tenantRepositoryMock.Setup(x => x.GetByIdAsync(tenantId)).ReturnsAsync((TenantEm?)null);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, userId, true)).ReturnsAsync(userEm);

            var (tenant, user) = await _useCase.ExecuteAsync(tenantId, userId);

            Assert.Null(tenant);
            Assert.NotNull(user);
        }

        [Fact(DisplayName = "現在のユーザー情報取得に成功する (ユーザーが存在しない)")]
        public async Task ExecuteAsync_Should_Return_Null_User_When_Not_Found()
        {
            var tenantId = TenantId.New();
            var userId = UserId.New();
            var now = DateTimeOffset.UtcNow;
            var tenantEm = TenantEm.Create(tenantId, now, now, userId, userId, new TenantName("テナント1"));

            _tenantRepositoryMock.Setup(x => x.GetByIdAsync(tenantId)).ReturnsAsync(tenantEm);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, userId, true)).ReturnsAsync((UserEm?)null);

            var (tenant, user) = await _useCase.ExecuteAsync(tenantId, userId);

            Assert.NotNull(tenant);
            Assert.Null(user);
        }
    }
}
