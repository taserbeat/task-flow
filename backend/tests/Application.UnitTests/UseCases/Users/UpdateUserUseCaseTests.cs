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
    /// <see cref="UpdateUserUseCase"/>のテスト
    /// </summary>
    public class UpdateUserUseCaseTests
    {
        private readonly Mock<IAuthorizeService> _authorizeServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<TimeProvider> _timeProviderMock;
        private readonly Mock<IPasswordHashService> _passwordHashServiceMock;
        private readonly UpdateUserUseCase _useCase;

        public UpdateUserUseCaseTests()
        {
            _authorizeServiceMock = new Mock<IAuthorizeService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _timeProviderMock = new Mock<TimeProvider>();
            _passwordHashServiceMock = new Mock<IPasswordHashService>();
            _useCase = new UpdateUserUseCase(_authorizeServiceMock.Object, _userRepositoryMock.Object, _roleRepositoryMock.Object, _uowMock.Object, _timeProviderMock.Object, _passwordHashServiceMock.Object);
        }

        [Fact(DisplayName = "ユーザー更新に成功する (メールアドレス)")]
        public async Task ExecuteAsync_Should_Success_When_Update_Email()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var targetId = UserId.New();
            var roleId = RoleId.New();
            var now = DateTimeOffset.UtcNow;
            var actorRoleEm = RoleEm.Create(roleId, RoleNameEnum.Admin, new RoleLabel("管理者"), RoleLevelEnum.Admin);
            var targetRoleEm = RoleEm.Create(roleId, RoleNameEnum.User, new RoleLabel("メンバー"), RoleLevelEnum.User);
            var targetUserEm = UserEm.Create(targetId, tenantId, now, now, actorId, actorId, new UserEmail("old@example.com"), new UserPasswordHash("hash"), new UserName("田中", "太郎"), roleId);
            targetUserEm.SetRole(targetRoleEm);
            var param = new UpdateUserParam { Email = "new@example.com" };

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(true);
            _userRepositoryMock.Setup(x => x.GetRoleByUserIdAsync(tenantId, actorId)).ReturnsAsync(actorRoleEm);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, targetId, true)).ReturnsAsync(targetUserEm);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, targetId, param);

            Assert.Equal("new@example.com", targetUserEm.Email.Value);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "ユーザー更新に成功する (有効化)")]
        public async Task ExecuteAsync_Should_Success_When_Activate()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var targetId = UserId.New();
            var roleId = RoleId.New();
            var now = DateTimeOffset.UtcNow;
            var actorRoleEm = RoleEm.Create(roleId, RoleNameEnum.Admin, new RoleLabel("管理者"), RoleLevelEnum.Admin);
            var targetRoleEm = RoleEm.Create(roleId, RoleNameEnum.User, new RoleLabel("メンバー"), RoleLevelEnum.User);
            var targetUserEm = UserEm.Create(targetId, tenantId, now, now, actorId, actorId, new UserEmail("test@example.com"), new UserPasswordHash("hash"), new UserName("田中", "太郎"), roleId);
            targetUserEm.SetRole(targetRoleEm);
            targetUserEm.Deactivate(now, actorId);
            var param = new UpdateUserParam { IsActive = true };

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(true);
            _userRepositoryMock.Setup(x => x.GetRoleByUserIdAsync(tenantId, actorId)).ReturnsAsync(actorRoleEm);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, targetId, true)).ReturnsAsync(targetUserEm);
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, targetId, param);

            Assert.True(targetUserEm.IsActive);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "ユーザー更新に失敗する (権限不足)")]
        public async Task ExecuteAsync_Should_Fail_When_Unauthorized()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var targetId = UserId.New();
            var param = new UpdateUserParam { Email = "new@example.com" };

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(false);

            await Assert.ThrowsAsync<AppForbiddenException>(() => _useCase.ExecuteAsync(tenantId, actorId, targetId, param));

            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        [Fact(DisplayName = "ユーザー更新に失敗する (対象ユーザーが存在しない)")]
        public async Task ExecuteAsync_Should_Fail_When_Target_User_Not_Found()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var targetId = UserId.New();
            var roleId = RoleId.New();
            var actorRoleEm = RoleEm.Create(roleId, RoleNameEnum.Admin, new RoleLabel("管理者"), RoleLevelEnum.Admin);
            var param = new UpdateUserParam { Email = "new@example.com" };

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(true);
            _userRepositoryMock.Setup(x => x.GetRoleByUserIdAsync(tenantId, actorId)).ReturnsAsync(actorRoleEm);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, targetId, true)).ReturnsAsync((UserEm?)null);

            await Assert.ThrowsAsync<AppValidateException>(() => _useCase.ExecuteAsync(tenantId, actorId, targetId, param));
        }

        [Fact(DisplayName = "ユーザー更新に失敗する (実行者のロールレベルが不足)")]
        public async Task ExecuteAsync_Should_Fail_When_Actor_Role_Level_Insufficient()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var targetId = UserId.New();
            var actorRoleId = RoleId.New();
            var targetRoleId = RoleId.New();
            var now = DateTimeOffset.UtcNow;
            var actorRoleEm = RoleEm.Create(actorRoleId, RoleNameEnum.User, new RoleLabel("メンバー"), RoleLevelEnum.User);
            var targetRoleEm = RoleEm.Create(targetRoleId, RoleNameEnum.Admin, new RoleLabel("管理者"), RoleLevelEnum.Admin);
            var targetUserEm = UserEm.Create(targetId, tenantId, now, now, actorId, actorId, new UserEmail("test@example.com"), new UserPasswordHash("hash"), new UserName("田中", "太郎"), targetRoleId);
            targetUserEm.SetRole(targetRoleEm);
            var param = new UpdateUserParam { Email = "new@example.com" };

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(true);
            _userRepositoryMock.Setup(x => x.GetRoleByUserIdAsync(tenantId, actorId)).ReturnsAsync(actorRoleEm);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(tenantId, targetId, true)).ReturnsAsync(targetUserEm);

            await Assert.ThrowsAsync<AppForbiddenException>(() => _useCase.ExecuteAsync(tenantId, actorId, targetId, param));
        }
    }
}
