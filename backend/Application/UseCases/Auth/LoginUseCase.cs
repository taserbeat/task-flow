using Application.Repositories;
using Application.Services;
using Domain.Entities.Auth;
using Domain.Entities.Roles;
using Domain.Entities.Tenants;
using Domain.Entities.Users;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Auth
{
    /// <summary>
    /// ログインのユースケース
    /// </summary>
    public class LoginUseCase
    {
        private readonly ILogger<LoginUseCase> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHashService _passwordHashService;

        public LoginUseCase(ILogger<LoginUseCase> logger, IUserRepository userRepository, IUnitOfWork uow, IPasswordHashService passwordHashService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _uow = uow;
            _passwordHashService = passwordHashService;
        }

        /// <summary>
        /// ログインを実行する
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="AppValidateException" />
        /// <returns></returns>
        public async Task<LoginResult> ExecuteAsync(LoginRequest request)
        {
            // ユーザーを取得
            UserEm? userEm;
            using (_uow.CreateBypassScope())
            {
                // NOTE:
                // 認証処理ではどのテナントのユーザー情報でも取得できる必要があるため、RLSをバイパスする (テナントを横断)
                userEm = await _userRepository.GetByEmailAsync(request.Email);
            }

            if (userEm is null)
            {
                throw new AppValidateException("メールアドレスまたはパスワードが間違っています。");
            }

            // パスワードを検証
            if (!_passwordHashService.VerifyPassword(request.Password, userEm.PasswordHash))
            {
                throw new AppValidateException("メールアドレスまたはパスワードが間違っています。");
            }

            var sessionId = SessionId.New();

            return new LoginResult(
                TenantId: userEm.TenantId,
                UserId: userEm.Id,
                Email: userEm.Email,
                RoleId: userEm.RoleId,
                RoleName: userEm.Role.Name,
                RoleLevel: userEm.Role.Level,
                SessionId: sessionId
            );
        }
    }

    /// <summary>
    /// ログインの入力
    /// </summary>
    /// <param name="Email"></param>
    /// <param name="Password"></param>
    /// <returns></returns>
    public record LoginRequest(UserEmail Email, string Password);

    /// <summary>
    /// ログインの出力
    /// </summary>
    /// <param name="TenantId">テナントID</param>
    /// <param name="UserId">ユーザーID</param>
    /// <param name="Email">メールアドレス</param>
    /// <param name="RoleId">ロールID</param>
    /// <param name="RoleName">ロール名</param>
    /// <param name="RoleLevel">ロールレベル</param>
    /// <param name="SessionId">セッションID</param>
    /// <returns></returns>
    public record LoginResult(
        TenantId TenantId,
        UserId UserId,
        UserEmail Email,
        RoleId RoleId,
        RoleNameEnum RoleName,
        RoleLevelEnum RoleLevel,
        SessionId SessionId
    );
}