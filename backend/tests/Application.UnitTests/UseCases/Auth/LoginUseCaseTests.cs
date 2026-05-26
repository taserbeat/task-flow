using Application.Repositories;
using Application.Services;
using Application.UseCases.Auth;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.UnitTests.UseCases.Auth
{
    /// <summary>
    /// <see cref="LoginUseCase"/>のテスト
    /// </summary>
    public class LoginUseCaseTests
    {
        private readonly Mock<ILogger<LoginUseCase>> _loggerMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IPasswordHashService> _passwordHashServiceMock;
        private readonly LoginUseCase _useCase;

        public LoginUseCaseTests()
        {
            _loggerMock = new Mock<ILogger<LoginUseCase>>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _passwordHashServiceMock = new Mock<IPasswordHashService>();
            _useCase = new LoginUseCase(_loggerMock.Object, _userRepositoryMock.Object, _uowMock.Object, _passwordHashServiceMock.Object);
        }

        [Fact(DisplayName = "ログインに成功する")]
        public async Task ExecuteAsync_Should_Success()
        {
            var email = new UserEmail("test@example.com");
            var password = "password123";
            var tenantId = TenantId.New();
            var userId = UserId.New();
            var roleId = RoleId.New();
            var passwordHash = new UserPasswordHash("hashedPassword");

            var roleEm = RoleEm.Create(roleId, RoleNameEnum.Admin, new RoleLabel("管理者"), RoleLevelEnum.Admin);
            var now = DateTimeOffset.UtcNow;
            var userEm = UserEm.Create(userId, tenantId, now, now, userId, userId, email, passwordHash, new UserName("田中", "太郎"), roleId);
            userEm.SetRole(roleEm);

            _uowMock.Setup(x => x.CreateBypassScope()).Returns(Mock.Of<IDisposable>());
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(userEm);
            _passwordHashServiceMock.Setup(x => x.VerifyPassword(password, passwordHash)).Returns(true);

            var request = new LoginRequest(email, password);
            var result = await _useCase.ExecuteAsync(request);

            Assert.Equal(tenantId, result.TenantId);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(email, result.Email);
            Assert.Equal(roleId, result.RoleId);
            Assert.Equal(RoleNameEnum.Admin, result.RoleName);
            Assert.Equal(RoleLevelEnum.Admin, result.RoleLevel);
        }

        [Fact(DisplayName = "ログインに失敗する (ユーザーが存在しない)")]
        public async Task ExecuteAsync_Should_Fail_When_User_Not_Found()
        {
            var email = new UserEmail("notfound@example.com");
            var password = "password123";

            _uowMock.Setup(x => x.CreateBypassScope()).Returns(Mock.Of<IDisposable>());
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync((UserEm?)null);

            var request = new LoginRequest(email, password);

            await Assert.ThrowsAsync<AppValidateException>(() => _useCase.ExecuteAsync(request));
        }

        [Fact(DisplayName = "ログインに失敗する (パスワードが間違っている)")]
        public async Task ExecuteAsync_Should_Fail_When_Password_Invalid()
        {
            var email = new UserEmail("test@example.com");
            var password = "wrongPassword";
            var tenantId = TenantId.New();
            var userId = UserId.New();
            var roleId = RoleId.New();
            var passwordHash = new UserPasswordHash("hashedPassword");
            var now = DateTimeOffset.UtcNow;

            var roleEm = RoleEm.Create(roleId, RoleNameEnum.Admin, new RoleLabel("管理者"), RoleLevelEnum.Admin);
            var userEm = UserEm.Create(userId, tenantId, now, now, userId, userId, email, passwordHash, new UserName("田中", "太郎"), roleId);
            userEm.SetRole(roleEm);

            _uowMock.Setup(x => x.CreateBypassScope()).Returns(Mock.Of<IDisposable>());
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(userEm);
            _passwordHashServiceMock.Setup(x => x.VerifyPassword(password, passwordHash)).Returns(false);

            var request = new LoginRequest(email, password);

            await Assert.ThrowsAsync<AppValidateException>(() => _useCase.ExecuteAsync(request));
        }
    }
}
