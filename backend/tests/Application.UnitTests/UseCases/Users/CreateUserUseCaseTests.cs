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
    /// <see cref="CreateUserUseCase"/>のテスト
    /// </summary>
    public class CreateUserUseCaseTests
    {
        private readonly Mock<IAuthorizeService> _authorizeServiceMock;
        private readonly Mock<IRoleService> _roleServiceMock;
        private readonly Mock<TimeProvider> _timeProviderMock;
        private readonly Mock<IPasswordHashService> _passwordHashServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly CreateUserUseCase _useCase;

        public CreateUserUseCaseTests()
        {
            _authorizeServiceMock = new Mock<IAuthorizeService>();
            _roleServiceMock = new Mock<IRoleService>();
            _timeProviderMock = new Mock<TimeProvider>();
            _passwordHashServiceMock = new Mock<IPasswordHashService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _useCase = new CreateUserUseCase(_authorizeServiceMock.Object, _roleServiceMock.Object, _timeProviderMock.Object, _passwordHashServiceMock.Object, _userRepositoryMock.Object, _uowMock.Object);
        }

        [Fact(DisplayName = "ユーザー作成に成功する")]
        public async Task ExecuteAsync_Should_Success()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var roleId = RoleId.New();
            var now = DateTimeOffset.UtcNow;
            var param = new CreateUserParam
            {
                Email = "test@example.com",
                Password = "password123",
                LastName = "田中",
                FirstName = "太郎",
                RoleId = roleId.Value
            };

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(true);
            _roleServiceMock.Setup(x => x.CanCreateUserAsync(tenantId, actorId, It.IsAny<RoleId>())).ReturnsAsync(true);
            _passwordHashServiceMock.Setup(x => x.GenerateHash(param.Password)).Returns(new UserPasswordHash("hashedPassword"));
            _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(now);
            _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<UserEm>())).Returns(Task.CompletedTask);
            _uowMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            await _useCase.ExecuteAsync(tenantId, actorId, param);

            _userRepositoryMock.Verify(x => x.AddAsync(It.Is<UserEm>(u => u.Email.Value == "test@example.com")), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact(DisplayName = "ユーザー作成に失敗する (権限不足)")]
        public async Task ExecuteAsync_Should_Fail_When_Unauthorized()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var roleId = RoleId.New();
            var param = new CreateUserParam
            {
                Email = "test@example.com",
                Password = "password123",
                LastName = "田中",
                FirstName = "太郎",
                RoleId = roleId.Value
            };

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(false);

            await Assert.ThrowsAsync<AppForbiddenException>(() => _useCase.ExecuteAsync(tenantId, actorId, param));

            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserEm>()), Times.Never);
        }

        [Fact(DisplayName = "ユーザー作成に失敗する (ロールレベルが不足)")]
        public async Task ExecuteAsync_Should_Fail_When_Role_Level_Insufficient()
        {
            var tenantId = TenantId.New();
            var actorId = UserId.New();
            var roleId = RoleId.New();
            var param = new CreateUserParam
            {
                Email = "test@example.com",
                Password = "password123",
                LastName = "田中",
                FirstName = "太郎",
                RoleId = roleId.Value
            };

            _authorizeServiceMock.Setup(x => x.HasRequiredRole(RoleLevelEnum.Admin)).Returns(true);
            _roleServiceMock.Setup(x => x.CanCreateUserAsync(tenantId, actorId, It.IsAny<RoleId>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<AppForbiddenException>(() => _useCase.ExecuteAsync(tenantId, actorId, param));

            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserEm>()), Times.Never);
        }
    }
}
