using Application.Repositories;
using Application.UseCases.Roles;
using Domain.Entities.Roles;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.UnitTests.UseCases.Roles
{
    /// <summary>
    /// <see cref="GetRolesUseCase"/>のテスト
    /// </summary>
    public class GetRolesUseCaseTests
    {
        private readonly Mock<ILogger<GetRolesUseCase>> _loggerMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly GetRolesUseCase _useCase;

        public GetRolesUseCaseTests()
        {
            _loggerMock = new Mock<ILogger<GetRolesUseCase>>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _useCase = new GetRolesUseCase(_loggerMock.Object, _roleRepositoryMock.Object);
        }

        [Fact(DisplayName = "ロール一覧取得に成功する")]
        public async Task Execute_Should_Return_Roles()
        {
            var roles = new List<RoleEm>
            {
                RoleEm.Create(RoleId.New(), RoleNameEnum.User, new RoleLabel("ユーザー"), RoleLevelEnum.User),
                RoleEm.Create(RoleId.New(), RoleNameEnum.Admin, new RoleLabel("管理者"), RoleLevelEnum.Admin),
                RoleEm.Create(RoleId.New(), RoleNameEnum.SystemAdmin, new RoleLabel("システム管理者"), RoleLevelEnum.SystemAdmin)
            };

            _roleRepositoryMock.Setup(x => x.GetRolesAsync()).ReturnsAsync(roles);

            var result = await _useCase.Execute();

            Assert.Equal(3, result.Count());
            Assert.Contains(result, r => r.Name == RoleNameEnum.User);
            Assert.Contains(result, r => r.Name == RoleNameEnum.Admin);
            Assert.Contains(result, r => r.Name == RoleNameEnum.SystemAdmin);
        }

        [Fact(DisplayName = "ロール一覧取得に成功する (空のリスト)")]
        public async Task Execute_Should_Return_Empty_List()
        {
            _roleRepositoryMock.Setup(x => x.GetRolesAsync()).ReturnsAsync(new List<RoleEm>());

            var result = await _useCase.Execute();

            Assert.Empty(result);
        }
    }
}
